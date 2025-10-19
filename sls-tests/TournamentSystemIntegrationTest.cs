using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.Models;
using sls_repos.Repositories;
using AutoMapper;
using sls_borders.DTO.UserDto;
using sls_borders.DTO.EditionDto;
using sls_borders.DTO.TournamentDto;
using sls_borders.DTO.TeamDto;
using sls_borders.DTO.GameDto;
using sls_borders.Enums;

namespace Tests;

/// <summary>
/// Integration test for the complete tournament system workflow.
/// Tests the full lifecycle: users -> edition -> teams -> tournaments -> games -> rounds -> deactivation
/// </summary>
public class TournamentSystemIntegrationTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserRepo _userRepo;
    private readonly EditionRepo _editionRepo;
    private readonly TournamentRepo _tournamentRepo;
    private readonly TeamRepo _teamRepo;
    private readonly GameRepo _gameRepo;

    public TournamentSystemIntegrationTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<sls_borders.Mappings.UserProfile>();
            cfg.AddProfile<sls_borders.Mappings.EditionProfile>();
            cfg.AddProfile<sls_borders.Mappings.TournamentProfile>();
            cfg.AddProfile<sls_borders.Mappings.TeamProfile>();
            cfg.AddProfile<sls_borders.Mappings.GameProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _userRepo = new UserRepo(_context, _mapper);
        _editionRepo = new EditionRepo(_context);
        _tournamentRepo = new TournamentRepo(_context, _mapper);
        _teamRepo = new TeamRepo(_context, _mapper, null!); // ImageService not needed for this test
        _gameRepo = new GameRepo(_context, _mapper);
    }

    [Fact]
    public async Task TournamentSystem_CompleteWorkflow_ShouldSucceed()
    {
        // Step 1: Create 10 users
        var users = await CreateUsersAsync();
        Assert.Equal(10, users.Count);

        // Step 2: Create a new edition and activate it
        var edition = await CreateEditionAsync();
        Assert.NotNull(edition);
        Assert.True(edition.IsActive);

        // Step 3: Create 2 teams and assign users to them
        var teams = await CreateTeamsAndAssignUsersAsync(users);
        Assert.Equal(2, teams.Count);

        // Step 4: Join teams to the edition
        await JoinTeamsToEditionAsync(teams, edition.Id);

        // Step 5: Create 4 tournaments (2 Swiss, 2 Round Robin)
        var tournaments = await CreateTournamentsAsync();
        Assert.Equal(4, tournaments.Count);

        // Step 6: Activate the first Swiss tournament
        var swissTournament = tournaments.First(t => t.Type == TournamentType.Swiss);
        var activationResult = await _tournamentRepo.ActivateTournamentAsync(swissTournament.Id);
        Assert.True(activationResult);

        // Verify tournament is activated
        var activatedTournament = await _tournamentRepo.GetByIdAsync(swissTournament.Id);
        Assert.NotNull(activatedTournament);
        Assert.Equal(TournamentStatus.Ongoing, activatedTournament.Status);
        Assert.Equal(1, activatedTournament.Round);
        Assert.NotEmpty(activatedTournament.Games);

        // Step 7: Update game scores for the first round
        await UpdateGameScoresAsync(activatedTournament.Games.ToList());

        // Step 8: Advance to next round
        var advanceResult = await _tournamentRepo.AdvandeToNextRoundAsync(swissTournament.Id);
        Assert.True(advanceResult);

        // Verify round advancement
        var tournamentAfterAdvance = await _tournamentRepo.GetByIdAsync(swissTournament.Id);
        Assert.NotNull(tournamentAfterAdvance);
        Assert.Equal(2, tournamentAfterAdvance.Round);

        // Step 9: Update scores for second round
        var secondRoundGames = tournamentAfterAdvance.Games.Where(g => g.Round == 2).ToList();
        await UpdateGameScoresAsync(secondRoundGames);

        // Step 10: Deactivate the tournament
        var deactivationResult = await _tournamentRepo.DeactivateTournamentAsync(swissTournament.Id);
        Assert.True(deactivationResult);

        // Verify tournament is deactivated
        var deactivatedTournament = await _tournamentRepo.GetByIdAsync(swissTournament.Id);
        Assert.NotNull(deactivatedTournament);
        Assert.Equal(TournamentStatus.Finished, deactivatedTournament.Status);

        // Step 11: Test Round Robin tournament
        var roundRobinTournament = tournaments.First(t => t.Type == TournamentType.RoundRobin);
        var rrActivationResult = await _tournamentRepo.ActivateTournamentAsync(roundRobinTournament.Id);
        Assert.True(rrActivationResult);

        var activatedRR = await _tournamentRepo.GetByIdAsync(roundRobinTournament.Id);
        Assert.NotNull(activatedRR);
        Assert.Equal(TournamentStatus.Ongoing, activatedRR.Status);
        Assert.NotEmpty(activatedRR.Games);
    }

    [Fact]
    public async Task TournamentSystem_CannotActivateTwoTournaments_ShouldFail()
    {
        // Setup
        var users = await CreateUsersAsync();
        var edition = await CreateEditionAsync();
        var teams = await CreateTeamsAndAssignUsersAsync(users);
        await JoinTeamsToEditionAsync(teams, edition.Id);
        var tournaments = await CreateTournamentsAsync();

        // Activate first tournament
        var firstTournament = tournaments[0];
        await _tournamentRepo.ActivateTournamentAsync(firstTournament.Id);

        // Try to activate second tournament - should throw
        var secondTournament = tournaments[1];
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentRepo.ActivateTournamentAsync(secondTournament.Id)
        );
    }

    [Fact]
    public async Task TournamentSystem_CannotAdvanceWithUnfinishedGames_ShouldFail()
    {
        // Setup
        var users = await CreateUsersAsync();
        var edition = await CreateEditionAsync();
        var teams = await CreateTeamsAndAssignUsersAsync(users);
        await JoinTeamsToEditionAsync(teams, edition.Id);
        var tournaments = await CreateTournamentsAsync();

        // Activate tournament
        var tournament = tournaments.First(t => t.Type == TournamentType.Swiss);
        await _tournamentRepo.ActivateTournamentAsync(tournament.Id);

        // Try to advance without finishing games - should throw
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentRepo.AdvandeToNextRoundAsync(tournament.Id)
        );
    }

    private async Task<List<User>> CreateUsersAsync()
    {
        var users = new List<User>();
        for (int i = 1; i <= 10; i++)
        {
            var user = new User
            {
                Email = $"user{i}@test.com",
                Name = $"User{i}",
                Surname = $"Surname{i}",
                ProfileImg = $"https://example.com/avatar{i}.jpg",
                Role = Role.user,
                AccountActivated = true
            };

            var createdUser = await _userRepo.CreateAsync(user, "Password123!");
            users.Add(createdUser);
        }
        return users;
    }

    private async Task<Edition> CreateEditionAsync()
    {
        var edition = new Edition
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Color = "#FF5733",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
            Organizer = "Test Organizer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _editionRepo.CreateAsync(edition);
    }

    private async Task<List<Team>> CreateTeamsAndAssignUsersAsync(List<User> users)
    {
        var teams = new List<Team>();

        // Create Team 1
        var team1 = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Team Alpha",
            Short = "TMA",
            Address = "123 Alpha Street",
            CreatedAt = DateTime.UtcNow
        };
        var createdTeam1 = await _teamRepo.CreateAsync(team1);
        teams.Add(createdTeam1);

        // Assign first 5 users to Team 1 and mark them as in play
        for (int i = 0; i < 5; i++)
        {
            var user = await _context.Users.FindAsync(users[i].Id);
            if (user != null)
            {
                user.TeamId = createdTeam1.Id;
                user.IsInPlay = true;
                _context.Users.Update(user);
            }
        }
        await _context.SaveChangesAsync();

        // Create Team 2
        var team2 = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Team Beta",
            Short = "TMB",
            Address = "456 Beta Avenue",
            CreatedAt = DateTime.UtcNow
        };
        var createdTeam2 = await _teamRepo.CreateAsync(team2);
        teams.Add(createdTeam2);

        // Assign next 5 users to Team 2 and mark them as in play
        for (int i = 5; i < 10; i++)
        {
            var user = await _context.Users.FindAsync(users[i].Id);
            if (user != null)
            {
                user.TeamId = createdTeam2.Id;
                user.IsInPlay = true;
                _context.Users.Update(user);
            }
        }
        await _context.SaveChangesAsync();

        return teams;
    }

    private async Task JoinTeamsToEditionAsync(List<Team> teams, Guid editionId)
    {
        foreach (var team in teams)
        {
            await _teamRepo.JoinEditonAsync(team.Id, editionId);
        }
    }

    private async Task<List<Tournament>> CreateTournamentsAsync()
    {
        var tournaments = new List<Tournament>();

        // Create 2 Swiss tournaments
        for (int i = 1; i <= 2; i++)
        {
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                Name = $"Swiss Tournament {i}",
                Date = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(i * 7), DateTimeKind.Utc),
                Location = $"Location {i}",
                Description = $"Test Swiss Tournament {i}",
                Type = TournamentType.Swiss,
                Status = TournamentStatus.Upcoming
            };

            var createdTournament = await _tournamentRepo.CreateAsync(tournament);
            if (createdTournament != null)
            {
                tournaments.Add(createdTournament);
            }
        }

        // Create 2 Round Robin tournaments
        for (int i = 1; i <= 2; i++)
        {
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                Name = $"Round Robin Tournament {i}",
                Date = DateTime.SpecifyKind(DateTime.UtcNow.AddDays((i + 2) * 7), DateTimeKind.Utc),
                Location = $"Location {i + 2}",
                Description = $"Test Round Robin Tournament {i}",
                Type = TournamentType.RoundRobin,
                Status = TournamentStatus.Upcoming
            };

            var createdTournament = await _tournamentRepo.CreateAsync(tournament);
            if (createdTournament != null)
            {
                tournaments.Add(createdTournament);
            }
        }

        return tournaments;
    }

    private async Task UpdateGameScoresAsync(List<Game> games)
    {
        // Randomly assign scores to games (avoiding bye games)
        var random = new Random(42); // Fixed seed for reproducibility
        foreach (var game in games.Where(g => g.BlackPlayerId != Guid.Empty))
        {
            var scoreValue = random.Next(3);
            GameScore score = scoreValue switch
            {
                0 => GameScore.WhiteWin,
                1 => GameScore.Draw,
                _ => GameScore.BlackWin
            };

            var updateDto = new UpdateGameDto
            {
                Score = score,
                Round = game.Round,
                TournamentId = game.TournamentId,
                WhitePlayerId = game.WhitePlayerId,
                BlackPlayerId = game.BlackPlayerId,
                WhiteTeamId = game.WhiteTeamId,
                BlackTeamId = game.BlackTeamId
            };

            await _gameRepo.UpdateAsync(game.Id, updateDto);
        }

        // Handle bye games - they should automatically win
        foreach (var game in games.Where(g => g.BlackPlayerId == Guid.Empty))
        {
            var updateDto = new UpdateGameDto
            {
                Score = GameScore.WhiteWin,
                Round = game.Round,
                TournamentId = game.TournamentId,
                WhitePlayerId = game.WhitePlayerId,
                BlackPlayerId = game.BlackPlayerId,
                WhiteTeamId = game.WhiteTeamId,
                BlackTeamId = game.BlackTeamId
            };

            await _gameRepo.UpdateAsync(game.Id, updateDto);
        }
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

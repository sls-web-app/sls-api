using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TeamDto;
using sls_borders.Enums;
using sls_borders.Models;
using sls_repos.Repositories;

namespace Tests;

public class TeamPointsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly TeamRepo _teamRepo;

    public TeamPointsTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Team, GetTeamWithPointsDto>();
        });
        _mapper = config.CreateMapper();

        _teamRepo = new TeamRepo(_context, _mapper, null!);
    }

    [Fact]
    public async Task SmallPoints_And_BigPoints_Aggregation_Works_As_Implemented()
    {
        // Arrange: create edition and two teams
        var edition = new Edition
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Color = "#FF5733",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            Organizer = "TestOrg",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Editions.Add(edition);

        var teamA = new Team { Id = Guid.NewGuid(), Name = "TeamA", Short = "A", Address = "AddrA" };
        var teamB = new Team { Id = Guid.NewGuid(), Name = "TeamB", Short = "B", Address = "AddrB" };
        var teamC = new Team { Id = Guid.NewGuid(), Name = "TeamC", Short = "C", Address = "AddrC" };
        var teamD = new Team { Id = Guid.NewGuid(), Name = "TeamD", Short = "D", Address = "AddrD" };

        await _teamRepo.CreateAsync(teamA);
        await _teamRepo.CreateAsync(teamB);
        await _teamRepo.CreateAsync(teamC);
        await _teamRepo.CreateAsync(teamD);

        // Join teams to edition
        await _teamRepo.JoinEditonAsync(teamA.Id, edition.Id);
        await _teamRepo.JoinEditonAsync(teamB.Id, edition.Id);
        await _teamRepo.JoinEditonAsync(teamC.Id, edition.Id);
        await _teamRepo.JoinEditonAsync(teamD.Id, edition.Id);

        // Persist edition teams relation
        _context.Editions.Update(edition);
        await _context.SaveChangesAsync();

        // Tournament 1 (A beats B, C beats D)
        var t1 = new Tournament { Id = Guid.NewGuid(), EditionId = edition.Id, Type = TournamentType.Swiss };
        _context.Tournaments.Add(t1);

        // Games in tournament 1
        _context.Games.AddRange(new[]
        {
            new Game { Id = Guid.NewGuid(), TournamentId = t1.Id, WhiteTeamId = teamA.Id, BlackTeamId = teamB.Id, Score = GameScore.WhiteWin, Round = 1 },
            new Game { Id = Guid.NewGuid(), TournamentId = t1.Id, WhiteTeamId = teamC.Id, BlackTeamId = teamD.Id, Score = GameScore.WhiteWin, Round = 1 }
        });

        // Tournament 2 (A dominates)
        var t2 = new Tournament { Id = Guid.NewGuid(), EditionId = edition.Id, Type = TournamentType.Swiss };
        _context.Tournaments.Add(t2);

        // Games in tournament 2: A wins twice, D beats B
        _context.Games.AddRange(new[]
        {
            new Game { Id = Guid.NewGuid(), TournamentId = t2.Id, WhiteTeamId = teamA.Id, BlackTeamId = teamB.Id, Score = GameScore.WhiteWin, Round = 1 },
            new Game { Id = Guid.NewGuid(), TournamentId = t2.Id, WhiteTeamId = teamA.Id, BlackTeamId = teamC.Id, Score = GameScore.WhiteWin, Round = 2 },
            new Game { Id = Guid.NewGuid(), TournamentId = t2.Id, WhiteTeamId = teamD.Id, BlackTeamId = teamB.Id, Score = GameScore.WhiteWin, Round = 1 }
        });

        // Tournament 3 (B gets a win and a draw vs C)
        var t3 = new Tournament { Id = Guid.NewGuid(), EditionId = edition.Id, Type = TournamentType.Swiss };
        _context.Tournaments.Add(t3);
        _context.Games.AddRange(new[]
        {
            new Game { Id = Guid.NewGuid(), TournamentId = t3.Id, WhiteTeamId = teamB.Id, BlackTeamId = teamC.Id, Score = GameScore.WhiteWin, Round = 1 },
            new Game { Id = Guid.NewGuid(), TournamentId = t3.Id, WhiteTeamId = teamB.Id, BlackTeamId = teamC.Id, Score = GameScore.Draw, Round = 1 }
        });

        // Tournament 4 (mixed results)
        var t4 = new Tournament { Id = Guid.NewGuid(), EditionId = edition.Id, Type = TournamentType.Swiss };
        _context.Tournaments.Add(t4);
        _context.Games.AddRange(new[]
        {
            new Game { Id = Guid.NewGuid(), TournamentId = t4.Id, WhiteTeamId = teamD.Id, BlackTeamId = teamA.Id, Score = GameScore.Draw, Round = 1 },
            new Game { Id = Guid.NewGuid(), TournamentId = t4.Id, WhiteTeamId = teamB.Id, BlackTeamId = teamD.Id, Score = GameScore.BlackWin, Round = 1 },
            new Game { Id = Guid.NewGuid(), TournamentId = t4.Id, WhiteTeamId = teamC.Id, BlackTeamId = teamA.Id, Score = GameScore.WhiteWin, Round = 1 }
        });

        await _context.SaveChangesAsync();

        // Act
        var scores = await _teamRepo.GetTeamsScoreInEditionAsync(edition.Id);

        // Assert
        var a = scores.Teams.First(t => t.Id == teamA.Id);
        var b = scores.Teams.First(t => t.Id == teamB.Id);
        var c = scores.Teams.First(t => t.Id == teamC.Id);
        var d = scores.Teams.First(t => t.Id == teamD.Id);

        // Expected small points (weighted: win=2, draw=1):
        // A: t1(2) + t2(4) + t4(1) = 7
        // B: t3(3) = 3
        // C: t1(2) + t3(1) + t4(2) = 5
        // D: t2(2) + t4(1+2) = 5
        Assert.Equal(7, a.SmallPoints);
        Assert.Equal(3, b.SmallPoints);
        Assert.Equal(5, c.SmallPoints);
        Assert.Equal(5, d.SmallPoints);

        // Expected big points (per-tournament ranking, teamsCount=4):
        // t1: A4, C4, B2, D2
        // t2: A4, D3, B2, C2
        // t3: B4, C3, A2, D2
        // t4: D4, C3, A2, B1
        // Totals: A=12, B=9, C=12, D=11
        Assert.Equal(12, a.BigPoints);
        Assert.Equal(9, b.BigPoints);
        Assert.Equal(12, c.BigPoints);
        Assert.Equal(11, d.BigPoints);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

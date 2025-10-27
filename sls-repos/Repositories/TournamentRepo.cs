using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Enums;
using sls_utils.MatchingUtils;
using sls_borders.DTO.GameDto;
using sls_borders.DTO.UserDto;
using sls_utils.BuchholzUtils;

namespace sls_repos.Repositories;

public class TournamentRepo(ApplicationDbContext context, IMapper mapper) : ITournamentRepo
{
    public async Task<List<Tournament>> GetAllAsync()
    {
        return await context.Tournaments
            .Include(t => t.Games)
            .ToListAsync();
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await context.Tournaments
            .Include(t => t.Games)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tournament>?> GetAllCurrentEditionTournamentsAsync()
    {
        var currentEdition = await context.Editions.Where(e => e.IsActive).FirstOrDefaultAsync();

        if (currentEdition == null)
            return null;

        return await context.Tournaments
            .Where(t => t.EditionId == currentEdition.Id)
            .ToListAsync();
    }

    public async Task<Tournament?> CreateAsync(Tournament tournament)
    {
        var currentEdition = await context.Editions.Where(e => e.IsActive).FirstOrDefaultAsync();

        if (currentEdition == null)
            return null;

        tournament.EditionId = currentEdition.Id;

        context.Tournaments.Add(tournament);
        await context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament?> UpdateAsync(Guid id, UpdateTournamentDto tournamentDto)
    {
        var existingTournament = await context.Tournaments.FindAsync(id);

        if (existingTournament == null)
            return null;

        mapper.Map(tournamentDto, existingTournament);

        await context.SaveChangesAsync();
        return existingTournament;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tournament = await context.Tournaments.FindAsync(id);
        if (tournament == null)
            return false;

        context.Tournaments.Remove(tournament);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateTournamentAsync(Guid id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Edition)
            .ThenInclude(e => e.Teams)
            .ThenInclude(t => t.Users)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament == null)
            return false;

        var currentActiveTournament = await context.Tournaments
            .Where(t => t.EditionId == tournament.EditionId && t.Status == TournamentStatus.Ongoing)
            .FirstOrDefaultAsync();
        if (currentActiveTournament != null)
            throw new InvalidOperationException("There is already an ongoing tournament in the current edition.");

        var games = new List<Game>();

        if (tournament.Type == TournamentType.Swiss)
        {
            games = SwissMatcher.GenerateGamesForSwissTournament(tournament);
        }

        if (tournament.Type == TournamentType.RoundRobin)
        {
            var teams = tournament.Edition.Teams.ToList();
            int teamCount = teams.Count;
            if (teamCount < 2)
                throw new InvalidOperationException("At least two teams are required for a round robin tournament.");

            // If odd number of teams, add a dummy team for bye (not null, but with Guid.Empty)
            Team? byeTeam = null;
            if (teamCount % 2 != 0)
            {
                byeTeam = new Team { Id = Guid.Empty, Name = "BYE", Users = [] };
                teams.Add(byeTeam);
                teamCount++;
            }

            int rounds = teamCount - 1;
            // Prepare a working list for rotation
            var rotation = new List<Team>(teams);
            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < teamCount / 2; i++)
                {
                    var teamA = rotation[i];
                    var teamB = rotation[teamCount - 1 - i];
                    if (teamA.Id == Guid.Empty || teamB.Id == Guid.Empty)
                        continue; // skip BYE

                    int playerCount = Math.Min(teamA.Users.Count(u => u.IsInPlay), teamB.Users.Count(u => u.IsInPlay));
                    for (int p = 0; p < playerCount; p++)
                    {
                        var playerA = teamA.Users.ElementAtOrDefault(p);
                        var playerB = teamB.Users.ElementAtOrDefault(p);
                        if (playerA == null || playerB == null)
                            continue;
                        if (!(playerA.IsInPlay && playerB.IsInPlay))
                            continue;
                        games.Add(new Game
                        {
                            TournamentId = tournament.Id,
                            WhitePlayerId = playerA.Id,
                            BlackPlayerId = playerB.Id,
                            WhiteTeamId = teamA.Id,
                            BlackTeamId = teamB.Id,
                            Round = round + 1
                        });
                    }
                }
                // Rotate teams (except the first one)
                var last = rotation[teamCount - 1];
                rotation.RemoveAt(teamCount - 1);
                rotation.Insert(1, last);
            }
        }

        context.Games.AddRange(games);
        tournament.Status = TournamentStatus.Ongoing;
        tournament.Round = 1;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateTournamentAsync(Guid id)
    {
        var tournament = await context.Tournaments.FindAsync(id);
        if (tournament == null)
            return false;

        if (tournament.Status != TournamentStatus.Ongoing)
            throw new InvalidOperationException("Only ongoing tournaments can be deactivated.");

        tournament.Status = TournamentStatus.Finished;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<AdvanceToNextRoundDto?> AdvandeToNextRoundAsync(Guid id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Games)
            .Include(t => t.Edition)
            .ThenInclude(e => e.Teams)
            .ThenInclude(t => t.Users)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament == null)
            return null;

        if (tournament.Status != TournamentStatus.Ongoing || tournament.Round == null)
            throw new InvalidOperationException("Only ongoing tournaments can advance to the next round.");

        var currentRoundGames = tournament.Games.Where(g => g.Round == tournament.Round).ToList();

        if (currentRoundGames.Any(g => g.Score == null))
            throw new InvalidOperationException("Not all games in the current round are finished.");

        tournament.Round += 1;

        List<Game> games = [];
        if (tournament.Type == TournamentType.Swiss)
        {
            games = SwissMatcher.GenerateGamesForSwissTournament(tournament);
            context.Games.AddRange(games);
        }
        if(tournament.Type == TournamentType.RoundRobin)
        {
            games = tournament.Games.Where(g => g.Round == tournament.Round).ToList();
            if(games.Count == 0)
                throw new InvalidOperationException("No more rounds available in Round Robin tournament.");
        }

        await context.SaveChangesAsync();
        return new AdvanceToNextRoundDto
        {
            Games = mapper.Map<List<GetGameDto>>(games),
            Round = tournament.Round.Value
        };
    }

    public async Task<List<UserInPlay>?> GetUsersInTournamentAsync(Guid tournamentId)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Edition)
            .ThenInclude(e => e.Teams)
            .ThenInclude(t => t.Users)
            .ThenInclude(u => u.Team)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
            return null;

        var usersInPlay = tournament.Edition.Teams
            .SelectMany(team => team.Users)
            .Where(user => user.IsInPlay)
            .ToList();

        var userInPlayDtos = mapper.Map<List<UserInPlay>>(usersInPlay);

        foreach (var userDto in userInPlayDtos)
        {
            var userGames = await context.Games
                .Where(g => (g.WhitePlayerId == userDto.Id || g.BlackPlayerId == userDto.Id) && g.Score != null)
                .ToListAsync();

            userDto.Wins = userGames.Count(g =>
                (g.WhitePlayerId == userDto.Id && g.Score == GameScore.WhiteWin) ||
                (g.BlackPlayerId == userDto.Id && g.Score == GameScore.BlackWin));

            userDto.Draws = userGames.Count(g => g.Score == GameScore.Draw);

            userDto.Losses = userGames.Count(g =>
                (g.WhitePlayerId == userDto.Id && g.Score == GameScore.BlackWin) ||
                (g.BlackPlayerId == userDto.Id && g.Score == GameScore.WhiteWin));
        }

        foreach (var player in userInPlayDtos)
        {
            HashSet<Guid> opponents = new();
            var playerGames = await context.Games
                .Where(g => (g.WhitePlayerId == player.Id || g.BlackPlayerId == player.Id) && g.Score != null)
                .ToListAsync();

            foreach (var game in playerGames)
            {
                Guid opponentId = game.WhitePlayerId == player.Id ? game.BlackPlayerId : game.WhitePlayerId;
                opponents.Add(opponentId);
            }
            
            player.FullBuchholz = BuchholzCalculator.CalculateFullBuchholzScore(userInPlayDtos, opponents);
            player.MedianBuchholz = BuchholzCalculator.CalculateMedianBuchholzScore(userInPlayDtos, opponents);
        }

        return userInPlayDtos;
    }

    public async Task<bool> UndoLastRoundAsync(Guid id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Games)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament == null)
            return false;

        if (tournament.Status != TournamentStatus.Ongoing || tournament.Round == null || tournament.Round <= 1)
            throw new InvalidOperationException("Cannot undo round for this tournament.");

        var lastRoundGames = tournament.Games.Where(g => g.Round == tournament.Round).ToList();
        context.Games.RemoveRange(lastRoundGames);

        tournament.Round -= 1;

        await context.SaveChangesAsync();
        return true;
    }
}


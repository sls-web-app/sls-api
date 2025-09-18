using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Enums;

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
            games = PairSwissPlayers(tournament, 1);
        }

        if (tournament.Type == TournamentType.RoundRobin)
        {
            // Get all teams in the edition that have at least one player in play
            var teams = tournament.Edition.Teams.ToList();

            // Each team plays against every other team once per round
            int rounds = teams.Count - 1 > 0 ? teams.Count - 1 : 1;

            for (int round = 1; round <= rounds; round++)
            {
                for (int i = 0; i < teams.Count; i++)
                {
                    for (int j = i + 1; j < teams.Count; j++)
                    {
                        var teamA = teams[i];
                        var teamB = teams[j];

                        // Pick a random player in play from each team for this match
                        var teamAPlayer = teamA.Users.FirstOrDefault(u => u.IsInPlay);
                        var teamBPlayer = teamB.Users.FirstOrDefault(u => u.IsInPlay);

                        if (teamAPlayer == null || teamBPlayer == null)
                            continue; // Skip if no available player

                        games.Add(new Game
                        {
                            TournamentId = tournament.Id,
                            WhitePlayerId = teamAPlayer.Id,
                            BlackPlayerId = teamBPlayer.Id,
                            WhiteTeamId = teamA.Id,
                            BlackTeamId = teamB.Id,
                            Round = round
                        });
                    }
                }
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

    public async Task<bool> AdvandeToNextRoundAsync(Guid id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Games)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament == null)
            return false;

        if (tournament.Status != TournamentStatus.Ongoing || tournament.Round == null)
            throw new InvalidOperationException("Only ongoing tournaments can advance to the next round.");

        var currentRoundGames = tournament.Games.Where(g => g.Round == tournament.Round).ToList();

        if (currentRoundGames.Any(g => g.Score == null))
            throw new InvalidOperationException("Not all games in the current round are finished.");

        tournament.Round += 1;

        if (tournament.Type == TournamentType.Swiss)
        {
            var games = PairSwissPlayers(tournament, tournament.Round.Value);
            context.Games.AddRange(games);
        }

        await context.SaveChangesAsync();
        return true;
    }

    // A helper method to pair players using a simple Swiss system
    private List<Game> PairSwissPlayers(Tournament tournament, int round)
    {
        var players = tournament.Edition.Teams
            .SelectMany(t => t.Users)
            .Where(u => u.IsInPlay)
            .ToList();

        // Compute wins based on finished games (assumes win = 1 point).
        // For simplicity, a finished game (Score not null) is assumed to give 1 point to the winner.
        // Adjust this calculation if draws or other scoring details are needed.
        var wins = players.ToDictionary(
            p => p.Id, 
            p => tournament.Games
                    .Where(g => ((g.WhitePlayerId == p.Id) && g.Score == GameScore.WhiteWin) ||
                                ((g.BlackPlayerId == p.Id) && g.Score == GameScore.BlackWin))
                    .Count()
                +
                tournament.Games
                    .Where(g => (g.WhitePlayerId == p.Id || g.BlackPlayerId == p.Id) && g.Score == GameScore.Draw)
                    .Count() * 0.5
        );

        // For round 1, all players are zero; for later rounds sort by wins.
        if (round == 1)
            players = players.OrderBy(p => Random.Shared.Next()).ToList();
        else
            players = players.OrderByDescending(p => wins[p.Id]).ThenBy(p => Random.Shared.Next()).ToList();

        var games = new List<Game>();

        // While at least two players remain, pair them.
        while (players.Count > 1)
        {
            var player1 = players[0];
            players.RemoveAt(0);

            // Look for the first opponent from a different team if possible.
            int matchIndex = players.FindIndex(p => p.TeamId != player1.TeamId);
            if (matchIndex == -1)
                matchIndex = 0; // Fallback

            var player2 = players[matchIndex];
            players.RemoveAt(matchIndex);

            // Optionally randomize which player gets white.
            bool assignWhitePlayer1 = Random.Shared.Next(2) == 0;
            games.Add(new Game
            {
                TournamentId = tournament.Id,
                WhitePlayerId = assignWhitePlayer1 ? player1.Id : player2.Id,
                BlackPlayerId = assignWhitePlayer1 ? player2.Id : player1.Id,
                WhiteTeamId = assignWhitePlayer1 ? (player1.TeamId ?? Guid.Empty) : (player2.TeamId ?? Guid.Empty),
                BlackTeamId = assignWhitePlayer1 ? (player2.TeamId ?? Guid.Empty) : (player1.TeamId ?? Guid.Empty),
                Round = round
            });
        }

        // If an odd number of players remains, assign a bye (automatic win).
        if (players.Count == 1)
        {
            var byePlayer = players[0];
            players.RemoveAt(0);
            games.Add(new Game
            {
                TournamentId = tournament.Id,
                WhitePlayerId = byePlayer.Id,
                BlackPlayerId = Guid.Empty, // Indicates no opponent.
                WhiteTeamId = byePlayer.TeamId ?? Guid.Empty,
                BlackTeamId = Guid.Empty,
                Round = round,
                Score = GameScore.WhiteWin // Registers an automatic win.
            });
        }

        return games;
    }
}


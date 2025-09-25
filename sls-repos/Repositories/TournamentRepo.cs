using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Enums;
using sls_utils.MatchingUtils;

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
            var games = SwissMatcher.GenerateGamesForSwissTournament(tournament);
            context.Games.AddRange(games);
        }

        await context.SaveChangesAsync();
        return true;
    }
}


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

        if (tournament.Type == TournamentType.Swiss)
        {
            var players = tournament.Edition.Teams.SelectMany(t => t.Users).Where(u => u.IsInPlay).ToList();
            while (players.Count > 0)
            {
                var player1 = players[0];
                User? player2 = null;
                players.RemoveAt(0);
                var otherPlayers = players.Where(u => u.TeamId != player1.TeamId).ToList();
                if (otherPlayers.Count == 0)
                {
                    int player2Index = Random.Shared.Next(otherPlayers.Count);
                    player2 = otherPlayers[player2Index];
                    players.RemoveAt(player2Index);
                }
                else
                {
                    int player2Index = Random.Shared.Next(players.Count);
                    player2 = players[player2Index];
                    players.Remove(player2);
                }
            }
        }
        if (tournament.Type == TournamentType.RoundRobin)
        {
            
        }

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
}


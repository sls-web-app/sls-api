using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;

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
}


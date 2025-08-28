using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;

public class TournamentRepo(ApplicationDbContext context) : ITournamentRepo
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

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        context.Tournaments.Add(tournament);
        await context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament?> UpdateAsync(Guid id, UpdateTournamentDto updateDto)
    {
        var existingTournament = await context.Tournaments.FindAsync(id);

        if (existingTournament == null)
            return null;

        if(updateDto.Date.HasValue)
            existingTournament.Date = updateDto.Date.Value;
        if(updateDto.Round.HasValue)
            existingTournament.Round = updateDto.Round;
        if(updateDto.Status.HasValue)
            existingTournament.Status = updateDto.Status.Value;
        if(updateDto.OrganizingTeamId.HasValue)
            existingTournament.OrganizingTeamId = updateDto.OrganizingTeamId.Value;
        if(updateDto.EditionId.HasValue)
            existingTournament.EditionId = updateDto.EditionId.Value;

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


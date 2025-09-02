using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.EditionDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;
public class EditionRepo(ApplicationDbContext context) : IEditionRepo
{
    public async Task<List<Edition>> GetAllAsync()
    {
        return await context.Editions.ToListAsync();
    }

    public async Task<List<Edition>> GetAllRemainingEditionsForTeamAsync(Guid teamId)
    {
        return await context.Editions
            .Where(e => !e.Teams.Any(t => t.Id == teamId))
            .ToListAsync();
    }

    public async Task<Edition> CreateAsync(Edition edition)
    {
        context.Editions.Add(edition);
        await context.SaveChangesAsync();
        return edition;
    }

    public async Task<Edition?> GetByIdAsync(Guid id)
    {
        return await context.Editions.FindAsync(id);
    }
}

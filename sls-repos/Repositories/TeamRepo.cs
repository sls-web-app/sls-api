using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.Team;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;

public class TeamRepo(ApplicationDbContext context, IMapper mapper) : ITeamRepo
{
    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams
            .Include(t => t.Users)
            .Include(t => t.Tournaments)
            .Include(t => t.OrganizedTournaments)
            .ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await context.Teams
            .Include(t => t.Users)
            .Include(t => t.Tournaments)
            .Include(t => t.OrganizedTournaments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Team> CreateAsync(CreateTeamDto createTeamDto)
    {
        var team = mapper.Map<Team>(createTeamDto);

        context.Teams.Add(team);
        await context.SaveChangesAsync();

        return team;
    }

    public async Task<Team?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto)
    {
        var existingTeam = await context.Teams
            .Include(t => t.Users)
            .Include(t => t.Tournaments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTeam == null)
            return null;

        mapper.Map(updateTeamDto, existingTeam);

        if (updateTeamDto.UsersId != null)
        {
            var usersToUpdate = await context.Users
                .Where(u => updateTeamDto.UsersId.Contains(u.Id))
                .ToListAsync();
            existingTeam.Users = usersToUpdate;
        }

        if (updateTeamDto.TournamentsId != null)
        {
            var tournamentsToUpdate = await context.Tournaments
                .Where(t => updateTeamDto.TournamentsId.Contains(t.Id))
                .ToListAsync();
            existingTeam.Tournaments = tournamentsToUpdate;
        }

        await context.SaveChangesAsync();
        return existingTeam;
    }

    public async Task<Team> GetTeamTournamentsInfo(Guid teamId)
    {
        var team = await context.Teams
            .Include(t => t.Tournaments)
            .Include(t => t.OrganizedTournaments)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
            throw new KeyNotFoundException($"Team with ID {teamId} not found.");

        return team;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var team = await context.Teams.FindAsync(id);
        if (team == null) return false;

        context.Teams.Remove(team);
        await context.SaveChangesAsync();
        return true;
    }
}


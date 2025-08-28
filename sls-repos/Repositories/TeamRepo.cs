using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TeamDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;

public class TeamRepo(ApplicationDbContext context) : ITeamRepo
{
    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams.ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await context.Teams.FindAsync(id);
    }

    public async Task<Team> CreateAsync(Team team)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        return team;
    }

    public async Task<Team?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto)
    {
        var existingTeam = await context.Teams.FindAsync(id);

        if (existingTeam == null)
            return null;
        
        if(!string.IsNullOrEmpty(updateTeamDto.Name))
            existingTeam.Name = updateTeamDto.Name;
        
        if(!string.IsNullOrEmpty(updateTeamDto.Short))
            existingTeam.Short = updateTeamDto.Short;
        
        if(!string.IsNullOrEmpty(updateTeamDto.Address))
            existingTeam.Address = updateTeamDto.Address;
        
        if(!string.IsNullOrEmpty(updateTeamDto.Img))
            existingTeam.Img = updateTeamDto.Img;

        await context.SaveChangesAsync();
        return existingTeam;
    }

    public async Task<Team> GetTeamTournamentsInfo(Guid teamId)
    {
        var team = await context.Teams.FindAsync(teamId);

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


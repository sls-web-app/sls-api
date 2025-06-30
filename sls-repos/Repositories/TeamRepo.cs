using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.Team;
using sls_borders.Models;
using sls_borders.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sls_repos.Repositories
{
    public class TeamRepo(ApplicationDbContext context, IMapper mapper) : ITeamRepo
    {
        public async Task<List<GetTeamDto>> GetAllAsync()
        {
            var teams = await context.Teams
                .Include(t => t.Users)
                .Include(t => t.Tournaments)
                .Include(t => t.OrganizedTournaments)
                .ToListAsync();
            
            return mapper.Map<List<GetTeamDto>>(teams);
        }

        public async Task<GetTeamDto?> GetByIdAsync(Guid id)
        {
            var team = await context.Teams
                .Include(t => t.Users)
                .Include(t => t.Tournaments)
                .Include(t => t.OrganizedTournaments)
                .FirstOrDefaultAsync(t => t.Id == id);

            return team != null ? mapper.Map<GetTeamDto>(team) : null;
        }

        public async Task<GetTeamDto> CreateAsync(CreateTeamDto createTeamDto)
        {
            var team = mapper.Map<Team>(createTeamDto);

            context.Teams.Add(team);
            await context.SaveChangesAsync();

            return mapper.Map<GetTeamDto>(team);
        }

        public async Task<GetTeamDto?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto)
        {
            var existingTeam = await context.Teams
                .Include(t => t.Users)
                .Include(t => t.Tournaments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTeam == null)
            {
                return null;
            }

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
            return mapper.Map<GetTeamDto>(existingTeam);
        }
        
        public async Task<GetTeamTournamentsDto> GetTeamTournamentsInfo(Guid teamId)
        {
            var team = await context.Teams
                .Include(t => t.Tournaments)
                .Include(t => t.OrganizedTournaments)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                throw new KeyNotFoundException($"Team with ID {teamId} not found.");
            }

            return mapper.Map<GetTeamTournamentsDto>(team);
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
}
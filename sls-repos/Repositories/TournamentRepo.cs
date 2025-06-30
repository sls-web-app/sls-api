using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories
{
    public class TournamentRepo(ApplicationDbContext context, IMapper mapper) : ITournamentRepo
    {
        public async Task<List<GetTournamentDto>> GetAllAsync()
        {
            var tournaments = await context.Tournaments
                .Include(t => t.OrganizingTeam)
                .Include(t => t.Teams)
                .Include(t => t.Games)
                .ToListAsync();
            
            return mapper.Map<List<GetTournamentDto>>(tournaments);
        }

        public async Task<GetTournamentDto?> GetByIdAsync(Guid id)
        {
            var tournament = await context.Tournaments
                .Include(t => t.OrganizingTeam)
                .Include(t => t.Teams)
                .Include(t => t.Games)
                .FirstOrDefaultAsync(t => t.Id == id);

            return mapper.Map<GetTournamentDto>(tournament);
        }

        public async Task<GetTournamentDto> CreateAsync(CreateTournamentDto createDto)
        {
            var tournament = mapper.Map<Tournament>(createDto);

            // **FIX: Specify that the incoming date is UTC**
            tournament.Date = DateTime.SpecifyKind(tournament.Date, DateTimeKind.Utc);

            var organizingTeam = await context.Teams.FindAsync(createDto.OrganizingTeamId);
            if (organizingTeam == null)
            {
                throw new KeyNotFoundException($"The team with ID {createDto.OrganizingTeamId} was not found.");
            }
            tournament.OrganizingTeam = organizingTeam;

            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();

            return mapper.Map<GetTournamentDto>(tournament);
        }

        public async Task<GetTournamentDto?> UpdateAsync(Guid id, UpdateTournamentDto updateDto)
        {
            var existingTournament = await context.Tournaments
                .Include(t => t.OrganizingTeam)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTournament == null)
            {
                return null;
            }

            mapper.Map(updateDto, existingTournament);
            
            // **FIX: Specify that the updated date is UTC**
            existingTournament.Date = DateTime.SpecifyKind(existingTournament.Date, DateTimeKind.Utc);
            
            if (existingTournament.OrganizingTeamId != updateDto.OrganizingTeamId)
            {
                var newOrganizingTeam = await context.Teams.FindAsync(updateDto.OrganizingTeamId);
                if (newOrganizingTeam == null)
                {
                    throw new KeyNotFoundException($"The team with ID {updateDto.OrganizingTeamId} was not found.");
                }
                existingTournament.OrganizingTeam = newOrganizingTeam;
            }
            
            await context.SaveChangesAsync();
            return mapper.Map<GetTournamentDto>(existingTournament);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var tournament = await context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return false;
            }

            context.Tournaments.Remove(tournament);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
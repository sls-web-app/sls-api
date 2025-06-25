using sls_borders.Models;
using sls_borders.Repositories;
using sls_repos.Data;
using Microsoft.EntityFrameworkCore;

namespace sls_repos.Repositories
{
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

        public async Task<Team?> UpdateAsync(Guid id, Team newTeamData)
        {
            var existingTeam = await context.Teams.FindAsync(id);
            if (existingTeam == null) return null;

            context.Entry(existingTeam).CurrentValues.SetValues(newTeamData);

            context.Teams.Update(existingTeam);
            await context.SaveChangesAsync();
            return existingTeam;
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
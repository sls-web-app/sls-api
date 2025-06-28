using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;
using Microsoft.EntityFrameworkCore;

namespace sls_repos.Repositories
{
    public class TournamentRepo(ApplicationDbContext context) : ITournamentRepo
    {
        public async Task<List<Tournament>> GetAllAsync()
        {
            return await context.Tournaments.ToListAsync();
        }

        public async Task<Tournament?> GetByIdAsync(Guid id)
        {
            return await context.Tournaments.FindAsync(id);
        }

        public async Task<Tournament> CreateAsync(Tournament tournament)
        {
            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();
            return tournament;
        }

        public async Task<Tournament?> UpdateAsync(Guid id, Tournament newTournamentData)
        {
            var existingTournament = await context.Tournaments.FindAsync(id);
            if (existingTournament == null) return null;

            context.Entry(existingTournament).CurrentValues.SetValues(newTournamentData);

            context.Tournaments.Update(existingTournament);
            await context.SaveChangesAsync();
            return existingTournament;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var tournament = await context.Tournaments.FindAsync(id);
            if (tournament == null) return false;

            context.Tournaments.Remove(tournament);
            await context.SaveChangesAsync();
            return true;
        }
    }
}

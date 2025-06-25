using sls_borders.Models;
using sls_borders.Repositories;
using sls_repos.Data;
using Microsoft.EntityFrameworkCore;

namespace sls_repos.Repositories
{
    public class GameRepo(ApplicationDbContext context) : IGameRepo
    {
        public async Task<List<Game>> GetAllAsync()
        {
            return await context.Games.ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            return await context.Games.FindAsync(id);
        }

        public async Task<Game> CreateAsync(Game game)
        {
            context.Games.Add(game);
            await context.SaveChangesAsync();
            return game;
        }

        public async Task<Game?> UpdateAsync(Guid id, Game newGameData)
        {
            var existingGame = await context.Games.FindAsync(id);
            if (existingGame == null) return null;

            context.Entry(existingGame).CurrentValues.SetValues(newGameData);

            context.Games.Update(existingGame);
            await context.SaveChangesAsync();
            return existingGame;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var game = await context.Games.FindAsync(id);
            if (game == null) return false;

            context.Games.Remove(game);
            await context.SaveChangesAsync();
            return true;
        }
    }
}

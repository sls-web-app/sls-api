using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.Game;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;

namespace sls_repos.Repositories;
// sls_repos/Repositories/GameRepo.cs
public class GameRepo(ApplicationDbContext context, IMapper mapper) : IGameRepo
{
    public async Task<List<Game>> GetAllAsync()
    {
        return await context.Games
            .Include(g => g.Tournament)
            .ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await context.Games
            .Include(g => g.Tournament)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game> CreateAsync(CreateGameDto gameDto)
    {
        var tournament = await context.Tournaments.FindAsync(gameDto.TournamentId);
        if (tournament == null)
            throw new InvalidOperationException($"Tournament with ID {gameDto.TournamentId} not found.");

        var whitePlayer = await context.Users.FindAsync(gameDto.WhitePlayerId);
        if (whitePlayer == null)
            throw new InvalidOperationException($"White player with ID {gameDto.WhitePlayerId} not found.");

        var blackPlayer = await context.Users.FindAsync(gameDto.BlackPlayerId);
        if (blackPlayer == null)
            throw new InvalidOperationException($"Black player with ID {gameDto.BlackPlayerId} not found.");

        var game = mapper.Map<Game>(gameDto);
        game.Id = Guid.NewGuid();

        context.Games.Add(game);
        await context.SaveChangesAsync();

        return await context.Games
            .Include(g => g.Tournament)
            .FirstAsync(g => g.Id == game.Id);
    }

    public async Task<Game?> UpdateAsync(Guid id, UpdateGameDto gameDto)
    {
        var existingGame = await context.Games.FindAsync(id);
        if (existingGame == null) return null;

        if (existingGame.TournamentId != gameDto.TournamentId)
        {
            var tournament = await context.Tournaments.FindAsync(gameDto.TournamentId);
            if (tournament == null)
                throw new InvalidOperationException($"Tournament with ID {gameDto.TournamentId} not found.");
        }

        if (existingGame.WhitePlayerId != gameDto.WhitePlayerId)
        {
            var whitePlayer = await context.Users.FindAsync(gameDto.WhitePlayerId);
            if (whitePlayer == null)
                throw new InvalidOperationException($"White player with ID {gameDto.WhitePlayerId} not found.");
        }

        if (existingGame.BlackPlayerId != gameDto.BlackPlayerId)
        {
            var blackPlayer = await context.Users.FindAsync(gameDto.BlackPlayerId);
            if (blackPlayer == null)
                throw new InvalidOperationException($"Black player with ID {gameDto.BlackPlayerId} not found.");
        }

        mapper.Map(gameDto, existingGame);
        await context.SaveChangesAsync();

        return await context.Games
            .Include(g => g.Tournament)
            .FirstAsync(g => g.Id == id);
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
    
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.Game;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;

namespace sls_repos.Repositories
{
    public class GameRepo : IGameRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GameRepo(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GetGameDto>> GetAllAsync()
        {
            var games = await _context.Games
                .Include(g => g.Tournament)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .ToListAsync();
                
            return _mapper.Map<List<GetGameDto>>(games);
        }

        public async Task<GetGameDto?> GetByIdAsync(Guid id)
        {
            var game = await _context.Games
                .Include(g => g.Tournament)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .FirstOrDefaultAsync(g => g.Id == id);

            return game != null ? _mapper.Map<GetGameDto>(game) : null;
        }

        public async Task<GetGameDto> CreateAsync(CreateGameDto gameDto)
        {
            var tournament = await _context.Tournaments.FindAsync(gameDto.TournamentId);
            if (tournament == null)
                throw new InvalidOperationException($"Tournament with ID {gameDto.TournamentId} not found.");
            
            var whitePlayer = await _context.Users.FindAsync(gameDto.WhitePlayerId);
            if (whitePlayer == null)
                throw new InvalidOperationException($"White player with ID {gameDto.WhitePlayerId} not found.");

            var blackPlayer = await _context.Users.FindAsync(gameDto.BlackPlayerId);
            if (blackPlayer == null)
                throw new InvalidOperationException($"Black player with ID {gameDto.BlackPlayerId} not found.");
            
            var game = _mapper.Map<Game>(gameDto);
            game.Id = Guid.NewGuid();

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            
            var createdGame = await _context.Games
                .Include(g => g.Tournament)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .FirstAsync(g => g.Id == game.Id);

            return _mapper.Map<GetGameDto>(createdGame);
        }

        public async Task<GetGameDto?> UpdateAsync(Guid id, UpdateGameDto gameDto)
        {
            var existingGame = await _context.Games.FindAsync(id);
            if (existingGame == null) return null;
            
            if (existingGame.TournamentId != gameDto.TournamentId)
            {
                var tournament = await _context.Tournaments.FindAsync(gameDto.TournamentId);
                if (tournament == null)
                    throw new InvalidOperationException($"Tournament with ID {gameDto.TournamentId} not found.");
            }
            
            if (existingGame.WhitePlayerId != gameDto.WhitePlayerId)
            {
                var whitePlayer = await _context.Users.FindAsync(gameDto.WhitePlayerId);
                if (whitePlayer == null)
                    throw new InvalidOperationException($"White player with ID {gameDto.WhitePlayerId} not found.");
            }

            if (existingGame.BlackPlayerId != gameDto.BlackPlayerId)
            {
                var blackPlayer = await _context.Users.FindAsync(gameDto.BlackPlayerId);
                if (blackPlayer == null)
                    throw new InvalidOperationException($"Black player with ID {gameDto.BlackPlayerId} not found.");
            }
            
            _mapper.Map(gameDto, existingGame);
            await _context.SaveChangesAsync();
            
            var updatedGame = await _context.Games
                .Include(g => g.Tournament)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .FirstAsync(g => g.Id == id);

            return _mapper.Map<GetGameDto>(updatedGame);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
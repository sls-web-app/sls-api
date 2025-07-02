using sls_borders.DTO.Game;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IGameRepo
    {
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(Guid id);
        Task<Game> CreateAsync(CreateGameDto game);
        Task<Game?> UpdateAsync(Guid id, UpdateGameDto game);
        Task<bool> DeleteAsync(Guid id);
    }
}
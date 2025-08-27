using sls_borders.DTO.GameDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IGameRepo
    {
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(Guid id);
        Task<Game> CreateAsync(Game game);
        Task<Game?> UpdateAsync(Guid id, UpdateGameDto game);
        Task<bool> DeleteAsync(Guid id);
    }
}
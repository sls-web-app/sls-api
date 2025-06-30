using sls_borders.DTO.Game;

namespace sls_borders.Repositories
{
    public interface IGameRepo
    {
        Task<List<GetGameDto>> GetAllAsync();
        Task<GetGameDto?> GetByIdAsync(Guid id);
        Task<GetGameDto> CreateAsync(CreateGameDto game);
        Task<GetGameDto?> UpdateAsync(Guid id, UpdateGameDto game);
        Task<bool> DeleteAsync(Guid id);
    }
}
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITournamentRepo
    {
        Task<List<GetTournamentDto>> GetAllAsync();
        Task<GetTournamentDto?> GetByIdAsync(Guid id);
        Task<GetTournamentDto> CreateAsync(CreateTournamentDto tournament);
        Task<GetTournamentDto?> UpdateAsync(Guid id, UpdateTournamentDto tournament);
        Task<bool> DeleteAsync(Guid id);
    }
}
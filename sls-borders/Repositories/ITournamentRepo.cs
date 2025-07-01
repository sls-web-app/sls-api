using sls_borders.DTO.TournamentDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITournamentRepo
    {
        Task<List<Tournament>> GetAllAsync();
        Task<Tournament?> GetByIdAsync(Guid id);
        Task<Tournament> CreateAsync(CreateTournamentDto tournament);
        Task<Tournament?> UpdateAsync(Guid id, UpdateTournamentDto tournament);
        Task<bool> DeleteAsync(Guid id);
    }
}
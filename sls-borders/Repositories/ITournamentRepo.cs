using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITournamentRepo
    {
        Task<List<Tournament>> GetAllAsync();
        Task<Tournament?> GetByIdAsync(Guid id);
        Task<Tournament> CreateAsync(Tournament tournament);
        Task<Tournament?> UpdateAsync(Guid id, Tournament tournament);
        Task<bool> DeleteAsync(Guid id);
    }
}
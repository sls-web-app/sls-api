using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IEditionRepo
    {
        Task<List<Edition>> GetAllAsync();
        Task<List<Edition>> GetAllRemainingEditionsForTeamAsync(Guid teamId);
        Task<Edition> GetActivatedEditionAsync();
        Task<Edition> CreateAsync(Edition edition);
        Task<Edition?> GetByIdAsync(Guid id);
    }
}

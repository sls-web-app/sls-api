using sls_borders.Models;

namespace sls_borders.Repositories
{

    public interface IAdminRepo
    {
        Task<List<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin> CreateAsync(Admin admin);
        Task<Admin?> UpdateAsync(Guid id, Admin admin);
        Task<bool> DeleteAsync(Guid id);
    }
}
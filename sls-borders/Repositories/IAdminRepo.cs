using sls_borders.DTO.Admin;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IAdminRepo
    {
        Task<List<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin> CreateAsync(CreateAdminDto newAdmin);
        Task<Admin?> UpdateAsync(Guid id, UpdateAdminDto adminDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
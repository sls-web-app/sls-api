using sls_borders.DTO.Admin;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IAdminRepo
    {
        Task<List<GetAdminDto>> GetAllAsync();
        Task<GetAdminDto?> GetByIdAsync(Guid id);
        Task<GetAdminDto> CreateAsync(CreateAdminDto newAdmin);
        Task<GetAdminDto?> UpdateAsync(Guid id, UpdateAdminDto adminDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
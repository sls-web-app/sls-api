using sls_borders.DTO.UserDto;

namespace sls_borders.Repositories
{
    public interface IUserRepo
    {
        Task<List<GetUserDto>> GetAllAsync();
        Task<GetUserDto?> GetByIdAsync(Guid id);
        Task<GetUserDto> CreateAsync(CreateUserDto getUserDto);
        Task<GetUserDto?> UpdateAsync(Guid id, UpdateUserDto getUserDto);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> EmailExistsAsync(string email);
        Task<GetUserDto?> GetByEmailAsync(string email);
    }
}
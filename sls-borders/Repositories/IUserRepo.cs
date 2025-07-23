using sls_borders.DTO.UserDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IUserRepo
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(CreateUserDto getUserDto, IEmailRepo emailRepo);
        Task<User?> UpdateAsync(Guid id, UpdateUserDto getUserDto);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> LoginAsync(string email, string password);
    }
}
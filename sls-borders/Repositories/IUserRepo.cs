using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IUserRepo
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(Guid id, User user);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByEmailAsync(string email);
    }
}
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface IUserInviteRepo
    {
        Task<List<UserInvite>> GetAllAsync();
        Task<UserInvite?> GetByIdAsync(Guid id);
        Task<UserInvite> CreateAsync(UserInvite userInvite);
        // Task<UserInvite?> UpdateAsync(Guid id, UserInvite userInvite);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> EmailExistsAsync(string email);
    }
}
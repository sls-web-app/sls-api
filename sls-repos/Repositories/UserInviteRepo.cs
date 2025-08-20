using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories;

public class UserInviteRepo(ApplicationDbContext context) : IUserInviteRepo
{
    public async Task<List<UserInvite>> GetAllAsync()
    {
        return await context.UserInvites.ToListAsync();
    }

    public async Task<UserInvite?> GetByIdAsync(Guid id)
    {
        return await context.UserInvites.FirstOrDefaultAsync(ui => ui.Id == id);
    }

    public async Task<UserInvite> CreateAsync(UserInvite userInvite)
    {
        context.UserInvites.Add(userInvite);
        await context.SaveChangesAsync();
        return userInvite;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invite = await GetByIdAsync(id);
        if (invite == null) return false;

        context.UserInvites.Remove(invite);
        await context.SaveChangesAsync();
        return true;
    }
}
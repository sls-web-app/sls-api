using sls_borders.Models;
using sls_borders.Repositories;
using sls_repos.Data;
using Microsoft.EntityFrameworkCore;

namespace sls_repos.Repositories
{
    public class UserRepo(ApplicationDbContext context) : IUserRepo
    {
        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(Guid id, User newUserData)
        {
            var existingUser = await context.Users.FindAsync(id);
            if (existingUser == null) return null;

            context.Entry(existingUser).CurrentValues.SetValues(newUserData);

            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) return false;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
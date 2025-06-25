using sls_borders.Models;
using sls_borders.Repositories;
using sls_repos.Data;
using Microsoft.EntityFrameworkCore;


namespace sls_repos.Repositories
{
    public class AdminRepo(ApplicationDbContext context) : IAdminRepo
    {
        public async Task<List<Admin>> GetAllAsync()
        {
            return await context.Admins.ToListAsync();
        }
        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await context.Admins.FindAsync(id);
        }
        public async Task<Admin> CreateAsync(Admin admin)
        {
            context.Admins.Add(admin);
            await context.SaveChangesAsync();
            return admin;
        }
        public async Task<Admin?> UpdateAsync(Guid id, Admin newAdminData)
        {
            var existingAdmin = await context.Admins.FindAsync(id);
            if (existingAdmin == null) return null;

            context.Entry(existingAdmin).CurrentValues.SetValues(newAdminData);

            context.Admins.Update(existingAdmin);
            await context.SaveChangesAsync();
            return existingAdmin;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var admin = await context.Admins.FindAsync(id);
            if (admin == null) return false;

            context.Admins.Remove(admin);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
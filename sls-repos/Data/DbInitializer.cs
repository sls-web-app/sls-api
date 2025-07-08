using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.Models;
using sls_utils.AuthUtils;

namespace sls_repos.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed data if necessary
            if (!await context.Admins.AnyAsync(ad => ad.Username == "admin"))
            {
                string Username = "admin";
                string Password = "Admin123!"; // Replace with a secure password

                (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(Password);

                // Create admin root user
                var admin = new Admin
                {
                    Username = Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}

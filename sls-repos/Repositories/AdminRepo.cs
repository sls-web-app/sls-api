using AutoMapper;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.Admin;
using System.Security.Cryptography;
using System.Text;

namespace sls_repos.Repositories;

public class AdminRepo(ApplicationDbContext context, IMapper mapper) : IAdminRepo
{
    public async Task<Admin?> LoginAsync(string username, string password)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        Console.WriteLine(username);
        Console.WriteLine(password);
        Console.WriteLine(admin?.Username);
        Console.WriteLine(admin?.PasswordHash);

        if (admin == null) return null;

        using var hmac = new HMACSHA512(Convert.FromBase64String(admin.PasswordSalt));
        Console.WriteLine(Convert.ToBase64String(hmac.Key));
        Console.WriteLine(admin.PasswordSalt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var computedHash = hmac.ComputeHash(passwordBytes);
        Console.WriteLine(Convert.ToBase64String(computedHash));

        if (Convert.ToBase64String(computedHash) != admin.PasswordHash) return null;

        return admin;
    }

    public async Task<List<Admin>> GetAllAsync()
    {
        return await context.Admins.ToListAsync();
    }

    public async Task<Admin?> GetByIdAsync(Guid id)
    {
        return await context.Admins.FindAsync(id);
    }

    public async Task<Admin> CreateAsync(CreateAdminDto newAdmin)
    {
        var existingAdmin = await context.Admins
            .FirstOrDefaultAsync(a => a.Username == newAdmin.Username);

        if (existingAdmin != null)
        {
            throw new InvalidOperationException($"Username '{newAdmin.Username}' is already taken.");
        }

        var admin = mapper.Map<Admin>(newAdmin);
        context.Admins.Add(admin);
        await context.SaveChangesAsync();
        return admin;
    }

    public async Task<Admin?> UpdateAsync(Guid id, UpdateAdminDto adminDto)
    {
        var existingAdmin = await context.Admins.FindAsync(id);
        if (existingAdmin == null) return null;

        if (existingAdmin.Username != adminDto.Username)
        {
            var usernameExists = await context.Admins
                .AnyAsync(a => a.Username == adminDto.Username && a.Id != id);

            if (usernameExists)
            {
                throw new InvalidOperationException($"Username '{adminDto.Username}' is already taken.");
            }
        }

        existingAdmin.Username = adminDto.Username;

        if (!string.IsNullOrEmpty(adminDto.Password))
        {
            using var hmac = new HMACSHA512();
            var saltBytes = hmac.Key;
            var passwordBytes = Encoding.UTF8.GetBytes(adminDto.Password);
            var hashBytes = hmac.ComputeHash(passwordBytes);

            existingAdmin.PasswordHash = Convert.ToBase64String(hashBytes);
            existingAdmin.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

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


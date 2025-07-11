using AutoMapper;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.Admin;
using System.Security.Cryptography;
using System.Text;
using sls_utils.AuthUtils;

namespace sls_repos.Repositories;

public class AdminRepo(ApplicationDbContext context, IMapper mapper) : IAdminRepo
{
    public async Task<Admin?> LoginAsync(string username, string password)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Username == username);

        if (admin == null) return null;

        string computedHash = HashingUtils.HashPassword(password, admin.PasswordSalt).Hash;
        if (computedHash != admin.PasswordHash) return null;

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
        (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(newAdmin.Password);
        admin.PasswordHash = passwordHash;
        admin.PasswordSalt = passwordSalt;
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
            (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(adminDto.Password);
            existingAdmin.PasswordHash = passwordHash;
            existingAdmin.PasswordSalt = passwordSalt;
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


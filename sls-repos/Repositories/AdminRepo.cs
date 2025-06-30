using AutoMapper;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.Admin;
using System.Security.Cryptography;
using System.Text;

namespace sls_repos.Repositories
{
    public class AdminRepo(ApplicationDbContext _context, IMapper _mapper) : IAdminRepo
    {

        public async Task<List<GetAdminDto>> GetAllAsync()
        {
            var admins = await _context.Admins.ToListAsync();
            return _mapper.Map<List<GetAdminDto>>(admins);
        }

        public async Task<GetAdminDto?> GetByIdAsync(Guid id)
        {
            var admin = await _context.Admins.FindAsync(id);
            return admin != null ? _mapper.Map<GetAdminDto>(admin) : null;
        }

        public async Task<GetAdminDto> CreateAsync(CreateAdminDto newAdmin)
        {
            var existingAdmin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == newAdmin.Username);
            
            if (existingAdmin != null)
            {
                throw new InvalidOperationException($"Username '{newAdmin.Username}' is already taken.");
            }

            var admin = _mapper.Map<Admin>(newAdmin);
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return _mapper.Map<GetAdminDto>(admin);
        }

        public async Task<GetAdminDto?> UpdateAsync(Guid id, UpdateAdminDto adminDto)
        {
            var existingAdmin = await _context.Admins.FindAsync(id);
            if (existingAdmin == null) return null;
            
            if (existingAdmin.Username != adminDto.Username)
            {
                var usernameExists = await _context.Admins
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

            await _context.SaveChangesAsync();
            return _mapper.Map<GetAdminDto>(existingAdmin);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return false;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
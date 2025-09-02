using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_utils.AuthUtils;

namespace sls_repos.Repositories;

public class UserRepo(ApplicationDbContext context, IMapper mapper) : IUserRepo
{
    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);

        if (user == null || !user.AccountActivated) return null;

        string computedHash = HashingUtils.HashPassword(password, user.PasswordSalt).Hash;
        if (computedHash != user.PasswordHash) return null;

        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.Team)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        var emailExists = await EmailExistsAsync(user.Email);
        if (emailExists)
            throw new InvalidOperationException($"Email '{user.Email}' is already in use.");

        (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(password);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await GetByIdAsync(id);

        if (existingUser == null) return null;

        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            var emailExists = await EmailExistsAsync(updateUserDto.Email);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already in use.");
            }
        }

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(updateUserDto.Password);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordSalt = passwordSalt;
        }

        mapper.Map(updateUserDto, existingUser);

        //context.Users.Update(existingUser);
        await context.SaveChangesAsync();

        return existingUser;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
            return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> ChangeEmailAsync(Guid userId, string newEmail)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            return null;

        var emailExists = await EmailExistsAsync(newEmail);
        if (emailExists)
            throw new InvalidOperationException($"Email '{newEmail}' is already in use.");

        user.Email = newEmail;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            return null;

        string currentHash = HashingUtils.HashPassword(currentPassword, user.PasswordSalt).Hash;
        if (currentHash != user.PasswordHash)
            throw new UnauthorizedAccessException("Current password is incorrect.");

        (string newHash, string newSalt) = HashingUtils.HashPassword(newPassword);
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(ui => ui.Invite)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> RegisterAsync(Guid userId, string password)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} was not found.");

        (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(password);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        user.AccountActivated = true;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeactivateAccountAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} was not found.");

        user.AccountActivated = false;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return true;
    }
}


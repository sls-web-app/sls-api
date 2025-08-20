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

        if (user == null) return null;

        string computedHash = HashingUtils.HashPassword(password, user.PasswordSalt).Hash;
        if (computedHash != user.PasswordHash) return null;

        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(CreateUserDto createUserDto)
    {
        var user = mapper.Map<User>(createUserDto);

        (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(createUserDto.Password);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Reload the user with related data
        return await context.Users
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstAsync(u => u.Id == user.Id);
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await GetByIdAsync(id);

        if (existingUser == null) return null;

        if (existingUser.Email != updateUserDto.Email)
        {
            var emailExists = await EmailExistsAsync(updateUserDto.Email);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already in use.");
            }
        }

        mapper.Map(updateUserDto, existingUser);

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(updateUserDto.Password);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordSalt = passwordSalt;
        }

        context.Users.Update(existingUser);
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

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
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
}


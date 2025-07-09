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
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(CreateUserDto createUserDto)
    {
        var user = mapper.Map<User>(createUserDto);

        if (createUserDto.TeamId != Guid.Empty)
        {
            var team = await context.Teams.FindAsync(createUserDto.TeamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {createUserDto.TeamId} was not found.");
            user.Team = team;
        }

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Reload the user with related data
        return await context.Users
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstAsync(u => u.Id == user.Id);
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await context.Users
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (existingUser == null)
            return null;

        mapper.Map(updateUserDto, existingUser);

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
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}


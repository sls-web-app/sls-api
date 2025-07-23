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

    public async Task<User> CreateAsync(CreateUserDto createUserDto, IEmailRepo emailRepo)
    {
        var user = mapper.Map<User>(createUserDto);

        if (createUserDto.TeamId != Guid.Empty)
        {
            var team = await context.Teams.FindAsync(createUserDto.TeamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {createUserDto.TeamId} was not found.");
            user.Team = team;
        }

        (string passwordHash, string passwordSalt) = HashingUtils.HashPassword(createUserDto.Password);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        await emailRepo.SendEmailAsync(user.Email, "Confirm Your Email",
            $@"
                <div style='font-family: Arial, sans-serif; background: #f9f9f9; padding: 30px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); padding: 32px;'>
                        <h2 style='color: #2d7ff9; text-align: center;'>Welcome to Minesweeper Battle!</h2>
                        <p style='font-size: 16px; color: #333; text-align: center;'>
                            Thank you for registering.<br>
                            Please confirm your email address to activate your account.
                        </p>
                        <div style='text-align: center; margin: 32px 0;'>
                            <a href='' 
                            style='background: #2d7ff9; color: #fff; text-decoration: none; padding: 14px 28px; border-radius: 5px; font-size: 16px; display: inline-block;'>
                                Confirm Email
                            </a>
                        </div>
                        <p style='font-size: 13px; color: #888; text-align: center;'>
                            If you did not create an account, you can safely ignore this email.
                        </p>
                    </div>
                </div>
                ");

        // Reload the user with related data
        return await context.Users
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstAsync(u => u.Id == user.Id);
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await GetByIdAsync(id);

        if (existingUser == null) return null;

        if(existingUser.Email != updateUserDto.Email)
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
            .Include(u => u.Team)
            .Include(u => u.GamesAsWhite)
            .Include(u => u.GamesAsBlack)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}


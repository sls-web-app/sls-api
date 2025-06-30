using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories
{
    public class UserRepo(ApplicationDbContext context, IMapper mapper) : IUserRepo
    {
        public async Task<List<GetUserDto>> GetAllAsync()
        {
            var users = await context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .ToListAsync();

            return mapper.Map<List<GetUserDto>>(users);
        }

        public async Task<GetUserDto?> GetByIdAsync(Guid id)
        {
            var user = await context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? mapper.Map<GetUserDto>(user) : null;
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto createUserDto)
        {
            var user = mapper.Map<User>(createUserDto);

            if (createUserDto.TeamId != Guid.Empty)
            {
                var team = await context.Teams.FindAsync(createUserDto.TeamId);
                if (team == null)
                    return null;
                user.Team = team;
            }

            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            

            // Reload the user with related data
            var createdUser = await context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstAsync(u => u.Id == user.Id);

            return mapper.Map<GetUserDto>(createdUser);
        }

        public async Task<GetUserDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
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

            return mapper.Map<GetUserDto>(existingUser);
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

        public async Task<GetUserDto?> GetByEmailAsync(string email)
        {
            var user = await context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstOrDefaultAsync(u => u.Email == email);

            return user != null ? mapper.Map<GetUserDto>(user) : null;
        }
    }
}
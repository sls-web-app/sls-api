using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_repos.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRepo(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GetUserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .ToListAsync();

            return _mapper.Map<List<GetUserDto>>(users);
        }

        public async Task<GetUserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? _mapper.Map<GetUserDto>(user) : null;
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);

            if (createUserDto.TeamId != Guid.Empty)
            {
                var team = await _context.Teams.FindAsync(createUserDto.TeamId);
                if (team == null)
                    return null;
                user.Team = team;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            

            // Reload the user with related data
            var createdUser = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstAsync(u => u.Id == user.Id);

            return _mapper.Map<GetUserDto>(createdUser);
        }

        public async Task<GetUserDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
                return null;

            _mapper.Map(updateUserDto, existingUser);

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetUserDto>(existingUser);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<GetUserDto?> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.GamesAsWhite)
                .Include(u => u.GamesAsBlack)
                .FirstOrDefaultAsync(u => u.Email == email);

            return user != null ? _mapper.Map<GetUserDto>(user) : null;
        }
    }
}
using sls_borders.Models;
using sls_borders.Repositories;
using sls_borders.Data;
using Microsoft.EntityFrameworkCore;
using sls_borders.DTO.UserDto;

namespace sls_repos.Repositories
{
    public class UserRepo(ApplicationDbContext context) : IUserRepo
    {
        public async Task<List<GetUserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<GetUserDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDto> CreateAsync(CreateUserDto getUserDto)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDto?> UpdateAsync(Guid id, UpdateUserDto getUserDto)
        {
            throw new NotImplementedException();
        }

        public async Task<GetUserDto> CreateAsync(GetUserDto getUserDto)
        {
            throw new NotImplementedException();
        }

        public async Task<GetUserDto?> UpdateAsync(Guid id, GetUserDto newGetUserDtoData)
        {
            var existingUser = await context.Users.FindAsync(id);
            if (existingUser == null) return null;

            context.Entry(existingUser).CurrentValues.SetValues(newGetUserDtoData);

            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) return false;

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
            throw new NotImplementedException();
        }

    }
}
using sls_borders.DTO.TeamDto;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITeamRepo
    {
        Task<List<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(Guid id);
        Task<Team> CreateAsync(Team team);
        Task<Team?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
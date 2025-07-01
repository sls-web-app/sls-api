using sls_borders.DTO.Team;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITeamRepo
    {
        Task<List<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(Guid id);
        Task<Team> CreateAsync(CreateTeamDto team);
        Task<Team?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto);
        Task<Team> GetTeamTournamentsInfo(Guid teamId);
        Task<bool> DeleteAsync(Guid id);
    }
}
using sls_borders.DTO.Team;
using sls_borders.Models;

namespace sls_borders.Repositories
{
    public interface ITeamRepo
    {
        Task<List<GetTeamDto>> GetAllAsync();
        Task<GetTeamDto?> GetByIdAsync(Guid id);
        Task<GetTeamDto> CreateAsync(CreateTeamDto team);
        Task<GetTeamDto?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto);
        Task<GetTeamTournamentsDto> GetTeamTournamentsInfo(Guid teamId);
        Task<bool> DeleteAsync(Guid id);
    }
}
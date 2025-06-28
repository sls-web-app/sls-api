using sls_borders.DTO.TournamentDto;
using sls_borders.DTO.UserDto;

namespace sls_borders.DTO.Team;

public class GetTeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Adress { get; set; } = null!;
    public string Img { get; set; } = null!;

    public ICollection<GetUserDto> Users { get; set; } = new List<GetUserDto>();
    public ICollection<GetTournamentDto> Tournaments { get; set; } = new List<GetTournamentDto>();
    public ICollection<GetTournamentDto> OrganizedTournaments { get; set; } = new List<GetTournamentDto>();
}
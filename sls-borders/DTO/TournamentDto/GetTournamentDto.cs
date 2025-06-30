using sls_borders.DTO.Game;
using sls_borders.DTO.Team;
using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

public class GetTournamentDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;
    public GetTeamDto OrganizingTeam { get; set; } = null!;

    public ICollection<GetTeamDto> Teams { get; set; } = new List<GetTeamDto>();
    public ICollection<GetGameDto> Games { get; set; } = new List<GetGameDto>();
}
using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

public class UpdateTournamentDto
{
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;
    public Guid EditionId { get; set; } = Guid.Empty;
}
using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for returning tournament information.
/// </summary>
public class GetTournamentDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;
    public Guid EditionId { get; set; } = Guid.Empty;
}
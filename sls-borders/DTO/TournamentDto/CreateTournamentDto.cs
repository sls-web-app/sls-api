using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for creating a new tournament.
/// </summary>
public class CreateTournamentDto
{
    public DateTime Date { get; set; }
    public TournamentType Type { get; set; }
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;
    public Guid EditionId { get; set; } = Guid.Empty;
}
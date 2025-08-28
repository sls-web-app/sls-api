using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for updating tournament information.
/// </summary>
public class UpdateTournamentDto
{
    public DateTime? Date { get; set; }
    public int? Round { get; set; }
    public TournamentStatus? Status { get; set; }
    public Guid? OrganizingTeamId { get; set; }
    public Guid? EditionId { get; set; }
}
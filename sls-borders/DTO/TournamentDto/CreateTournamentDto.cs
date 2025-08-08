using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

public class CreateTournamentDto
{
    public DateTime Date { get; set; }
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;
    public Guid EditionId { get; set; } = Guid.Empty;
}
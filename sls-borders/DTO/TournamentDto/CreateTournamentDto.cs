using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for creating a new tournament.
/// </summary>
public class CreateTournamentDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Round { get; set; }
    public string? Location { get; set; }
    public TournamentType Type { get; set; }
    public Guid EditionId { get; set; } = Guid.Empty;
}
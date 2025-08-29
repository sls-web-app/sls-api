using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for returning tournament information.
/// </summary>
public class GetTournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public string? Location { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
    public TournamentType Type { get; set; }
    public string? Description { get; set; }
}

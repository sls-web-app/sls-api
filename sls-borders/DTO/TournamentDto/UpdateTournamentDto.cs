using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for updating tournament information.
/// </summary>
public class UpdateTournamentDto
{
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public string? Location { get; set; }
    public TournamentStatus Status { get; set; }
    public TournamentType Type { get; set; }
    public string? Description { get; set; }
}
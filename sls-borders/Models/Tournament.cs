using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a tournament in the system.
/// </summary>
public class Tournament
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public string? Location { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
    public TournamentType Type { get; set; }

    public Guid EditionId { get; set; } = Guid.Empty;

    public Edition Edition { get; set; } = null!;
    public ICollection<Game> Games { get; set; } = [];
}

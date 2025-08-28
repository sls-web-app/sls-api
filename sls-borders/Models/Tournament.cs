using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a tournament in the system.
/// </summary>
public class Tournament
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int? Round { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;

    public Guid EditionId { get; set; } = Guid.Empty;
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;

    public Edition Edition { get; set; } = null!;
    public Team OrganizingTeam { get; set; } = null!;
    public ICollection<Game> Games { get; set; } = [];
}

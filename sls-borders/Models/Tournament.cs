using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a tournament in the system.
/// </summary>
public class Tournament
{
    /// <summary>
    /// Gets or sets the unique identifier of the tournament.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the tournament.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the round number of the tournament (optional).
    /// </summary>
    public int? Round { get; set; }

    /// <summary>
    /// Gets or sets the status of the tournament.
    /// </summary>
    public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;

    /// <summary>
    /// Gets or sets the identifier of the edition associated with the tournament.
    /// </summary>
    public Guid EditionId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the edition associated with the tournament.
    /// </summary>
    public Edition Edition { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of games in the tournament.
    /// </summary>
    public ICollection<Game> Games { get; set; } = [];
}

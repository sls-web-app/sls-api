using sls_borders.Enums;

namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for updating tournament information.
/// </summary>
public class UpdateTournamentDto
{
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
    /// Gets or sets the identifier of the organizing team.
    /// </summary>
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the edition.
    /// </summary>
    public Guid EditionId { get; set; }
}
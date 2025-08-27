namespace sls_borders.DTO.TournamentDto;

/// <summary>
/// Data Transfer Object for creating a new tournament.
/// </summary>
public class CreateTournamentDto
{
    /// <summary>
    /// Gets or sets the date of the tournament.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the organizing team.
    /// </summary>
    public Guid OrganizingTeamId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the edition.
    /// </summary>
    public Guid EditionId { get; set; } = Guid.Empty;
}
namespace sls_borders.DTO.GameDto;

/// <summary>
/// Data Transfer Object for creating a new game.
/// </summary>
public class CreateGameDto
{
    /// <summary>
    /// Gets or sets the tournament identifier.
    /// </summary>
    public Guid TournamentId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the white player.
    /// </summary>
    public Guid WhitePlayerId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the black player.
    /// </summary>
    public Guid BlackPlayerId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the white team.
    /// </summary>
    public Guid WhiteTeamId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the black team.
    /// </summary>
    public Guid BlackTeamId { get; set; } = Guid.Empty;
}
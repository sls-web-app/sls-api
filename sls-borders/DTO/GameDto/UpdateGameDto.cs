using sls_borders.Enums;

namespace sls_borders.DTO.GameDto;

/// <summary>
/// Data Transfer Object for updating game information.
/// </summary>
public class UpdateGameDto
{
    /// <summary>
    /// Gets or sets the round number of the game.
    /// </summary>
    public int? Round { get; set; }

    /// <summary>
    /// Gets or sets the score of the game.
    /// </summary>
    public GameScore? Score { get; set; }

    /// <summary>
    /// Gets or sets the tournament identifier.
    /// </summary>
    public Guid? TournamentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the white player.
    /// </summary>
    public Guid? WhitePlayerId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the black player.
    /// </summary>
    public Guid? BlackPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the white team.
    /// </summary>
    public Guid? WhiteTeamId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the black team.
    /// </summary>
    public Guid? BlackTeamId { get; set; }
}
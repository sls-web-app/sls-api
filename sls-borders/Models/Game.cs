using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a game played in a tournament.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the unique identifier of the game.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the round number of the game.
    /// </summary>
    public int Round { get; set; }

    /// <summary>
    /// Gets or sets the score of the game.
    /// </summary>
    public GameScore? Score { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the tournament.
    /// </summary>
    public Guid TournamentId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the white player.
    /// </summary>
    public Guid WhitePlayerId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the white team.
    /// </summary>
    public Guid WhiteTeamId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the black player.
    /// </summary>
    public Guid BlackPlayerId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the identifier of the black team.
    /// </summary>
    public Guid BlackTeamId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the tournament associated with the game.
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who played as white.
    /// </summary>
    public User WhitePlayer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the team of the white player.
    /// </summary>
    public Team WhiteTeam { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who played as black.
    /// </summary>
    public User BlackPlayer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the team of the black player.
    /// </summary>
    public Team BlackTeam { get; set; } = null!;
}

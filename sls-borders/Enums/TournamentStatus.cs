namespace sls_borders.Enums;

/// <summary>
/// Defines the various statuses a tournament can have.
/// </summary>
public enum TournamentStatus
{
    /// <summary>
    /// Indicates that the tournament is scheduled to start in the future.
    /// </summary>
    Upcoming,

    /// <summary>
    /// Indicates that the tournament is currently ongoing.
    /// </summary>
    Ongoing,

    /// <summary>
    /// Indicates that the tournament has concluded.
    /// </summary>
    Finished,
}
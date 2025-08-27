namespace sls_borders.Models;

/// <summary>
/// Represents a team in the system.
/// </summary>
public class Team
{
    /// <summary>
    /// Gets or sets the unique identifier of the team.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the short name or abbreviation of the team.
    /// </summary>
    public string Short { get; set; } = null!;

    /// <summary>
    /// Gets or sets the address of the team.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets the image URL of the team (optional).
    /// </summary>
    public string? Img { get; set; } = null!;

    /// <summary>
    /// Gets or sets the creation date and time of the team.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of users associated with the team.
    /// </summary>
    public ICollection<User> Users { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of editions associated with the team.
    /// </summary>
    public ICollection<Edition> Editions { get; set; } = [];
}

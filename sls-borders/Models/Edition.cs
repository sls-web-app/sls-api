namespace sls_borders.Models;

/// <summary>
/// Represents an edition of the event or competition.
/// </summary>
public class Edition
{
    /// <summary>
    /// Gets or sets the unique identifier of the edition.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the edition number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the color of the edition (hex color code).
    /// </summary>
    public string Color { get; set; } = null!; // Hex color code

    /// <summary>
    /// Gets or sets the start date of the edition.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the edition.
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the organizer of the edition.
    /// </summary>
    public string Organizer { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the edition is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the edition.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of teams associated with the edition.
    /// </summary>
    public ICollection<Team> Teams { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of tournaments associated with the edition.
    /// </summary>
    public ICollection<Tournament> Tournaments { get; set; } = [];
}

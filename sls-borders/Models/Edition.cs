namespace sls_borders.Models;

/// <summary>
/// Represents an edition of the event or competition.
/// </summary>
public class Edition
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Color { get; set; } = null!; // Hex color code
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Organizer { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Tournament> Tournaments { get; set; } = [];
}

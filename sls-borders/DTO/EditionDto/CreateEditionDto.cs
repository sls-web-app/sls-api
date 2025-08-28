namespace sls_borders.DTO.EditionDto;

/// <summary>
/// Data Transfer Object for creating a new edition.
/// </summary>
public class CreateEditionDto
{
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
}

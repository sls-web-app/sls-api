namespace sls_borders.DTO.EditionDto;

/// <summary>
/// Data Transfer Object for retrieving edition details.
/// </summary>
public class GetEditionDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the edition.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the edition.
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
    public bool IsActive { get; set; }
}

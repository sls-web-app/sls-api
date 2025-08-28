namespace sls_borders.DTO.EditionDto;

/// <summary>
/// Data Transfer Object for creating a new edition.
/// </summary>
public class CreateEditionDto
{
    public int Number { get; set; }
    public string Color { get; set; } = null!; // Hex color code
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Organizer { get; set; } = null!;
}

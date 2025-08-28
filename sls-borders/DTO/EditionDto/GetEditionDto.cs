namespace sls_borders.DTO.EditionDto;

/// <summary>
/// Data Transfer Object for retrieving edition details.
/// </summary>
public class GetEditionDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Color { get; set; } = null!; // Hex color code
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; }
}

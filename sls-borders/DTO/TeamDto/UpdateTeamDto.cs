namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for updating team information.
/// </summary>
public class UpdateTeamDto
{
    public string Name { get; set; } = null!;
    public string Short { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Img { get; set; }
}
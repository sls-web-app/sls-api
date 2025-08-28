namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for updating team information.
/// </summary>
public class UpdateTeamDto
{
    public string? Name { get; set; }
    public string? Short { get; set; }
    public string? Address { get; set; }
    public string? Img { get; set; }
}
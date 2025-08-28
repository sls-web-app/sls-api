namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for creating a new team.
/// </summary>
public class CreateTeamDto
{
    public string Name { get; set; } = null!;
    public string Short { get; set; } = null!;
    public string Address { get; set; } = null!;
}
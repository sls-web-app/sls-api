namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for creating a new team.
/// </summary>
public class CreateTeamDto
{
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
}
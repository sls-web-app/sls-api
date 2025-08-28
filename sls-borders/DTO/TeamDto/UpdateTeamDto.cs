namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for updating team information.
/// </summary>
public class UpdateTeamDto
{
    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the short name or abbreviation of the team.
    /// </summary>
    public string? Short { get; set; }

    /// <summary>
    /// Gets or sets the address of the team.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the image URL of the team.
    /// </summary>
    public string? Img { get; set; }
}
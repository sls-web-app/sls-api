using System.ComponentModel.DataAnnotations;

namespace sls_borders.DTO.Team;

/// <summary>
/// DTO do tworzenia nowego zespo�u.
/// </summary>
public class CreateTeamDto
{
    public string Name { get; set; } = null!;

    public string Short { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Img { get; set; }
}
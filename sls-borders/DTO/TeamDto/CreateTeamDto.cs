using System.ComponentModel.DataAnnotations;

namespace sls_borders.DTO.Team;

public class CreateTeamDto
{
    public string Name { get; set; } = null!;

    public string Short { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Img { get; set; }
}
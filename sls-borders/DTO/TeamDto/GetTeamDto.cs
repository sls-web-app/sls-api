namespace sls_borders.DTO.TeamDto;

/// <summary>
/// Data Transfer Object for returning team information.
/// </summary>
public class GetTeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Short { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Img { get; set; } = null!;
}
namespace sls_borders.DTO.TeamDto;

public class GetTeamWithPointsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Short { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Img { get; set; } = null!;
    public int BigPoints { get; set; }
    public int SmallPoints { get; set; }
}
namespace sls_borders.DTO.Team;

public class UpdateTeamDto
{
    public string Name { get; set; } = null!;
    public string Adress { get; set; } = null!;
    public string Img { get; set; } = null!;

    public ICollection<Guid> UsersId { get; set; } = new List<Guid>();
    public ICollection<Guid> TournamentsId { get; set; } = new List<Guid>();
}
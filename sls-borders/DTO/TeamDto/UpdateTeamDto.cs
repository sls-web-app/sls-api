namespace sls_borders.DTO.Team;

public class UpdateTeamDto
{
    public string Name { get; set; } = null!;
    public string Short { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Img { get; set; } = null!;

    public ICollection<Guid> UsersId { get; set; } = new List<Guid>();
    public ICollection<Guid> TournamentsId { get; set; } = new List<Guid>();
}
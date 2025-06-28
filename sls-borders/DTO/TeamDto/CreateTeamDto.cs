using sls_borders.DTO.UserDto;
using sls_borders.Models;

namespace sls_borders.DTO.Team;

public class CreateTeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Adress { get; set; } = null!;
    public string Img { get; set; } = null!;

    public ICollection<Guid> UsersId { get; set; } = new List<Guid>();
    public ICollection<Guid> TournamentsId { get; set; } = new List<Guid>();

}
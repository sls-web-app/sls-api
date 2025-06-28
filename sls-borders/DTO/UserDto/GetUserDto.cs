using sls_borders.DTO.Game;
using sls_borders.DTO.Team;
using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? ClassName { get; set; }
    public Role Role { get; set; } = Role.User;

    public Guid TeamId { get; set; } = Guid.Empty;
    public GetTeamDto Team { get; set; } = null!;
    public ICollection<GetGameDto> GamesAsWhite { get; set; } = new List<GetGameDto>();
    public ICollection<GetGameDto> GamesAsBlack { get; set; } = new List<GetGameDto>();
}
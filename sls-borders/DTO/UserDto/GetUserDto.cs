using sls_borders.DTO.TeamDto;
using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for returning user information.
/// </summary>
public class GetUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? ClassName { get; set; }
    public Role Role { get; set; }
    public bool AccountActivated { get; set; }
    public bool IsInPlay { get; set; }
    public bool IsLider { get; set; }

    public GetTeamDto? Team { get; set; }
}
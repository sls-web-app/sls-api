using sls_borders.DTO.Game;
using sls_borders.DTO.Team;
using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

public class CreateUserDto
{
    public required string Email { get; set; } 
    public required string Password{ get; set; } 
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? ClassName { get; set; }
    public Role Role { get; set; } = Role.User;
    public Guid TeamId { get; set; } = Guid.Empty;
}
using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for inviting a new user.
/// </summary>
public class InviteUserDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public Role Role { get; set; } = Role.User;
}

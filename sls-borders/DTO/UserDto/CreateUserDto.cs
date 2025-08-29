using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for creating a new user.
/// </summary>
public class CreateUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? ClassName { get; set; }
    public Role Role { get; set; } = Role.user;
    public Guid? TeamId { get; set; }
}
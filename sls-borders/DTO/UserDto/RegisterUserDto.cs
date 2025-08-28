namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for registering a new user.
/// </summary>
public class RegisterUserDto
{
    public string Password { get; set; } = null!;
}
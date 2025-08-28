namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for logging in a user.
/// </summary>
public class LoginUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
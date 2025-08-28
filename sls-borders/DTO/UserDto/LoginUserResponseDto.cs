namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for the response after logging in a user.
/// </summary>
public class LoginUserResponseDto
{
    public string Token { get; set; } = null!;
}
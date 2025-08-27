namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for the response after logging in a user.
/// </summary>
public class LoginUserResponseDto
{
    /// <summary>
    /// Gets or sets the JWT token for the logged-in user.
    /// </summary>
    public string Token { get; set; } = null!;
}
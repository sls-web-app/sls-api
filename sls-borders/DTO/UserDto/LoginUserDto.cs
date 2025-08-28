namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for logging in a user.
/// </summary>
public class LoginUserDto
{
    /// <summary>
    /// Gets or sets the email address of the invited user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password of the invited user.
    /// </summary>
    public string Password { get; set; } = null!;
}
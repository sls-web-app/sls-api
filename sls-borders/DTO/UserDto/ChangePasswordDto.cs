namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for changing a user's password.
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for admin login.
/// </summary>
public class LoginAdminDto
{
    /// <summary>
    /// Gets or sets the username of the admin.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password of the admin.
    /// </summary>
    public string Password { get; set; } = null!;
}
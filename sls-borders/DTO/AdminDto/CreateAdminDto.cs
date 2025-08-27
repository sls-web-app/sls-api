namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for creating an admin user.
/// </summary>
public class CreateAdminDto
{
    /// <summary>
    /// Gets or sets the username for the admin user.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for the admin user.
    /// </summary>
    public required string Password { get; set; }   
}
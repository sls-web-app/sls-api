namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for updating admin information.
/// </summary>
public class UpdateAdminDto
{
    /// <summary>
    /// Gets or sets the username of the admin.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the new password of the admin (optional).
    /// </summary>
    public string? Password { get; set; }
}
namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for returning admin information.
/// </summary>
public class GetAdminDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the admin.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username of the admin.
    /// </summary>
    public required string Username { get; set; } 
}
namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for updating admin information.
/// </summary>
public class UpdateAdminDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
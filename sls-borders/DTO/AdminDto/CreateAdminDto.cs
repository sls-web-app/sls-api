namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for creating an admin user.
/// </summary>
public class CreateAdminDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }   
}
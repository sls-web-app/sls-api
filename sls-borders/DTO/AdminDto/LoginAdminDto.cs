namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for admin login.
/// </summary>
public class LoginAdminDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
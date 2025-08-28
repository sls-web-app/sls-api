namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for the response after admin login.
/// </summary>
public class LoginAdminResponseDto
{
    /// <summary>
    /// Gets or sets the JWT token for the authenticated admin.
    /// </summary>
    public string Token { get; set; } = null!;
}
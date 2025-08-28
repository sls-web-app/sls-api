namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for the response after admin login.
/// </summary>
public class LoginAdminResponseDto
{
    public string Token { get; set; } = null!;
}
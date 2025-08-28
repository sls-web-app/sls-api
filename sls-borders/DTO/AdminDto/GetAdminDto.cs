namespace sls_borders.DTO.AdminDto;

/// <summary>
/// Data Transfer Object for returning admin information.
/// </summary>
public class GetAdminDto
{
    public required Guid Id { get; set; }
    public required string Username { get; set; } 
}
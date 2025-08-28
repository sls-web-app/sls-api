using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for updating a user.
/// </summary>
public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ProfileImg { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? ClassName { get; set; }
    public Role? Role { get; set; }
    public bool? AccountActivated { get; set; }
    public bool? IsInPlay { get; set; }
    public bool? IsLider { get; set; }

    public Guid? TeamId { get; set; }
}
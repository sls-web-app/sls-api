using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for inviting a new user.
/// </summary>
public class InviteUserDto
{
    /// <summary>
    /// Gets or sets the email address of the invited user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the first name of the invited user.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the surname of the invited user.
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Gets or sets the role of the invited user.
    /// </summary>
    public Role Role { get; set; } = Role.User;
}

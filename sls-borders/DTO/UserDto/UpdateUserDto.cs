using sls_borders.Enums;

namespace sls_borders.DTO.UserDto;

/// <summary>
/// Data Transfer Object for updating a user.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets or sets the profile image URL of the user.
    /// </summary>
    public string ProfileImg { get; set; } = null!;

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the surname of the user.
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Gets or sets the class name of the user.
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the role of the user.
    /// </summary>
    public Role Role { get; set; } = Role.User;

    /// <summary>
    /// Gets or sets the team ID of the user.
    /// </summary>
    public Guid TeamId { get; set; } = Guid.Empty;
}
using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = null!;

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
    /// Gets or sets the class name of the user (optional).
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the role of the user.
    /// </summary>
    public Role Role { get; set; } = Role.User;

    /// <summary>
    /// Gets or sets a value indicating whether the user's account is activated.
    /// </summary>
    public bool AccountActivated { get; set; } = false;

    /// <summary>
    /// Gets or sets the password hash for the user.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password salt for the user.
    /// </summary>
    public string PasswordSalt { get; set; } = null!;

    /// <summary>
    /// Gets or sets the team ID associated with the user (optional).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team associated with the user.
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// Gets or sets the collection of games where the user played as white.
    /// </summary>
    public ICollection<Game> GamesAsWhite { get; set; } = new List<Game>();

    /// <summary>
    /// Gets or sets the collection of games where the user played as black.
    /// </summary>
    public ICollection<Game> GamesAsBlack { get; set; } = new List<Game>();
}

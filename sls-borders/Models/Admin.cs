namespace sls_borders.Models;

/// <summary>
/// Represents an administrator in the system.
/// </summary>
public class Admin
{
    /// <summary>
    /// Gets or sets the unique identifier for the admin.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the username of the admin.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the hashed password of the admin.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the salt used for hashing the admin's password.
    /// </summary>
    public required string PasswordSalt { get; set; }
}

namespace sls_borders.Models;

/// <summary>
/// Represents an administrator in the system.
/// </summary>
public class Admin
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
}

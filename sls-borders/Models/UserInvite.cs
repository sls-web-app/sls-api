namespace sls_borders.Models;

/// <summary>
/// Represents a user invitation in the system.
/// </summary>
public class UserInvite
{
    public Guid Id { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
    public Guid UserId { get; set; } = Guid.Empty;
    public User User { get; set; } = null!;
}

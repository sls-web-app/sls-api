namespace sls_borders.Models;

/// <summary>
/// Represents a user invitation in the system.
/// </summary>
public class UserInvite
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
}

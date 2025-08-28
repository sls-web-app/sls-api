namespace sls_borders.Models;

/// <summary>
/// Represents a user invitation in the system.
/// </summary>
public class UserInvite
{
    /// <summary>
    /// Gets or sets the unique identifier for the user invite.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the email address of the invited user.
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;
}

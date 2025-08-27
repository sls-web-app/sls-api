namespace sls_borders.Enums;

/// <summary>
/// Defines the various roles a user can have within the system.
/// </summary>
public enum Role
{
    /// <summary>
    /// Indicates a standard user role.
    /// </summary>
    User,

    /// <summary>
    /// Indicates an observer role with limited permissions.
    /// </summary>
    Observer,

    /// <summary>
    /// Indicates an administrator role with elevated permissions.
    /// </summary>
    Admin
}

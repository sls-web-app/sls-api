using sls_borders.Enums;

namespace sls_borders.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string ProfileImg { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? ClassName { get; set; }
    public Role Role { get; set; } = Role.user;
    public bool AccountActivated { get; set; } = false;
    public bool IsInPlay { get; set; } = false;
    public bool IsLider { get; set; } = false;

    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public Guid? TeamId { get; set; }

    public Team? Team { get; set; }
    public UserInvite? Invite { get; set; }
    public ICollection<Game> GamesAsWhite { get; set; } = new List<Game>();
    public ICollection<Game> GamesAsBlack { get; set; } = new List<Game>();
}

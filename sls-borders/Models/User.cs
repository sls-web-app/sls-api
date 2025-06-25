using sls_borders.Enums;

namespace sls_borders.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public string ProfileImg { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ClassName { get; set; }
        public Role Role { get; set; } = Role.User;

        public Guid TeamId { get; set; } = Guid.Empty;
        public Team Team { get; set; } = null!;
        public ICollection<Game> GamesAsWhite { get; set; } = new List<Game>();
        public ICollection<Game> GamesAsBlack { get; set; } = new List<Game>();
    }
}

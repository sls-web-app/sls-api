using sls_borders.Enums;

namespace sls_borders.Models
{
    public class UserInvite
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public Role Role { get; set; } = Role.User;
        public Guid TeamId { get; set; } = Guid.Empty;
    }
}
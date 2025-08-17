using sls_borders.Enums;

namespace sls_borders.Models
{
    public class UserInvite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
    }
}
namespace sls_borders.Models
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
    }
}
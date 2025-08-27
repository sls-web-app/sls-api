using sls_borders.DTO.UserDto;

namespace sls_borders.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Short { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Img { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Edition> Editions { get; set; } = new List<Edition>();
    }
}
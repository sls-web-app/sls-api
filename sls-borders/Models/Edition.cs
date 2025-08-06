
namespace sls_borders.Models
{
    public class Edition
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Organizer { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    }
}

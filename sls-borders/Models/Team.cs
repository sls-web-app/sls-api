namespace sls_borders.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Adress { get; set; } = null!;
        public string Img { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    }
}
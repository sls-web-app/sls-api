using sls_borders.Enums;

namespace sls_borders.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Round { get; set; }
        public TournamentStatus Status { get; set; } = TournamentStatus.Upcoming;
        public Guid OrganizingTeamId { get; set; } = Guid.Empty;
        public Team OrganizingTeam { get; set; } = null!;

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
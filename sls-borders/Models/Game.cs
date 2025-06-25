using sls_borders.Enums;

namespace sls_borders.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public int Round { get; set; }
        public GameScore? Score { get; set; }

        public Guid TournamentId { get; set; } = Guid.Empty;
        public Tournament Tournament { get; set; } = null!;
        public Guid WhitePlayerId { get; set; } = Guid.Empty;
        public User WhitePlayer { get; set; } = null!;
        public Guid BlackPlayerId { get; set; } = Guid.Empty;
        public User BlackPlayer { get; set; } = null!;
    }
}
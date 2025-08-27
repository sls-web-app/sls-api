using sls_borders.DTO.UserDto;
using sls_borders.Enums;

namespace sls_borders.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public int Round { get; set; }
        public GameScore? Score { get; set; }

        public Guid TournamentId { get; set; } = Guid.Empty;
        public Guid WhitePlayerId { get; set; } = Guid.Empty;
        public Guid WhiteTeamId { get; set; } = Guid.Empty;
        public Guid BlackPlayerId { get; set; } = Guid.Empty;
        public Guid BlackTeamId { get; set; } = Guid.Empty;

        public Tournament Tournament { get; set; } = null!;
        public User WhitePlayer { get; set; } = null!;
        public Team WhiteTeam { get; set; } = null!;
        public User BlackPlayer { get; set; } = null!;
        public Team BlackTeam { get; set; } = null!;
    }
}
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
        public Tournament Tournament { get; set; } = null!;
        public Guid WhitePlayerId { get; set; } = Guid.Empty;
        public Guid BlackPlayerId { get; set; } = Guid.Empty;
    }
}
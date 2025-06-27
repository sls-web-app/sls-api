using sls_borders.Models;

namespace sls_borders.DTO.Game;

public class CreateGameDto
{
    public Guid TournamentId { get; set; } = Guid.Empty;
    public Tournament Tournament { get; set; } = null!;
    public Guid WhitePlayerId { get; set; } = Guid.Empty;
    public User WhitePlayer { get; set; } = null!;
    public Guid BlackPlayerId { get; set; } = Guid.Empty;
    public User BlackPlayer { get; set; } = null!;
}
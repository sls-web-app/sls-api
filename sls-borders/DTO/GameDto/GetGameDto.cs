using sls_borders.DTO.TournamentDto;
using sls_borders.DTO.UserDto;
using sls_borders.Enums;
using sls_borders.Models;

namespace sls_borders.DTO.Game;

public class GetGameDto
{
    public Guid Id { get; set; }
    public int Round { get; set; }
    public GameScore? Score { get; set; }

    public Guid TournamentId { get; set; } = Guid.Empty;
    public Guid WhitePlayerId { get; set; } = Guid.Empty;
    public Guid BlackPlayerId { get; set; } = Guid.Empty;
}
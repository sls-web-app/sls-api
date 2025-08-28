using sls_borders.Enums;

namespace sls_borders.DTO.GameDto;

/// <summary>
/// Data Transfer Object for returning game information.
/// </summary>
public class GetGameDto
{
    public Guid Id { get; set; }
    public int Round { get; set; }
    public GameScore? Score { get; set; }
    public Guid TournamentId { get; set; } = Guid.Empty;
    public Guid WhitePlayerId { get; set; } = Guid.Empty;
    public Guid BlackPlayerId { get; set; } = Guid.Empty;
    public Guid WhiteTeamId { get; set; } = Guid.Empty;
    public Guid BlackTeamId { get; set; } = Guid.Empty;
}
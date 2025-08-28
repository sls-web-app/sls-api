using sls_borders.Enums;

namespace sls_borders.DTO.GameDto;

/// <summary>
/// Data Transfer Object for updating game information.
/// </summary>
public class UpdateGameDto
{
    public int? Round { get; set; }
    public GameScore? Score { get; set; }
    public Guid? TournamentId { get; set; }
    public Guid? WhitePlayerId { get; set; }
    public Guid? BlackPlayerId { get; set; }
    public Guid? WhiteTeamId { get; set; }
    public Guid? BlackTeamId { get; set; }
}
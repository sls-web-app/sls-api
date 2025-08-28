namespace sls_borders.DTO.GameDto;

/// <summary>
/// Data Transfer Object for creating a new game.
/// </summary>
public class CreateGameDto
{
    public Guid TournamentId { get; set; } = Guid.Empty;
    public Guid WhitePlayerId { get; set; } = Guid.Empty;
    public Guid BlackPlayerId { get; set; } = Guid.Empty;
    public Guid WhiteTeamId { get; set; } = Guid.Empty;
    public Guid BlackTeamId { get; set; } = Guid.Empty;
}
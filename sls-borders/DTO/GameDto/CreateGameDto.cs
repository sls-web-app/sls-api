using sls_borders.DTO.TournamentDto;
using sls_borders.DTO.UserDto;
using sls_borders.Models;
using GetUserDto = sls_borders.DTO.UserDto.GetUserDto;

namespace sls_borders.DTO.Game;

public class CreateGameDto
{
    public Guid TournamentId { get; set; } = Guid.Empty;
    public GetTournamentDto Tournament { get; set; } = null!;
    public Guid WhitePlayerId { get; set; } = Guid.Empty;
    public GetUserDto WhitePlayer { get; set; } = null!;
    public Guid BlackPlayerId { get; set; } = Guid.Empty;
    public GetUserDto BlackPlayer { get; set; } = null!;
}
using sls_borders.DTO.GameDto;

public class AdvanceToNextRoundDto
{
    public required List<GetGameDto> Games { get; set; }
    public int Round { get; set; }
}
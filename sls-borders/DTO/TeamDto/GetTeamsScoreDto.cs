namespace sls_borders.DTO.TeamDto;

public class GetTeamsScoreDto
{
    public List<GetTeamWithPointsDto> Teams { get; set; } = new();
}
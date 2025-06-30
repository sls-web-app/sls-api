using sls_borders.DTO.TournamentDto;

namespace sls_borders.DTO.Team;

public class GetTeamTournamentsDto
{ 
        public ICollection<GetTournamentDto> Tournaments { get; set; } = new List<GetTournamentDto>();
        public ICollection<GetTournamentDto> OrganizedTournaments { get; set; } = new List<GetTournamentDto>();
}
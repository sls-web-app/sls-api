using AutoMapper;
using sls_borders.DTO.Team;
using sls_borders.Models;

namespace sls_borders.Mappings
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            // Maps from the Team entity to the GetTeamDto
            CreateMap<Team, GetTeamDto>();

            // Maps from the Team entity to the GetTeamTournamentsDto
            CreateMap<Team, GetTeamTournamentsDto>();

            // Maps from CreateTeamDto to the Team entity.
            // The repository is responsible for handling relationships, so we ignore them here.
            CreateMap<CreateTeamDto, Team>()
                .ForMember(dest => dest.EditionTeamMembers, opt => opt.Ignore());


            // Maps from UpdateTeamDto to the Team entity.
            // The repository should also handle updating relationships.
            CreateMap<UpdateTeamDto, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EditionTeamMembers, opt => opt.Ignore());
        }
    }
}
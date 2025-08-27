using AutoMapper;
using sls_borders.DTO.TeamDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Team entities and DTOs.
/// </summary>
public class TeamProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamProfile"/> class.
    /// </summary>
    public TeamProfile()
    {
        // Maps from the Team entity to the GetTeamDto
        CreateMap<Team, GetTeamDto>();

        // Maps from CreateTeamDto to the Team entity.
        // The repository is responsible for handling relationships and image, so we ignore them here.
        CreateMap<CreateTeamDto, Team>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Maps from UpdateTeamDto to the Team entity.
        // The repository should also handle updating relationships.
        CreateMap<UpdateTeamDto, Team>();
    }
}

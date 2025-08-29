using AutoMapper;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for Tournament-related mappings.
/// </summary>
public class TournamentProfile : Profile
{
    public TournamentProfile()
    {
        // Maps from the data transfer object for creation to the main entity.
        CreateMap<CreateTournamentDto, Tournament>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.TournamentStatus.Upcoming))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Date, DateTimeKind.Utc)));

        // Maps from the main entity to the data transfer object for retrieval.
        // This will automatically handle mapping the collections of Teams and Games.
        CreateMap<Tournament, GetTournamentDto>();

        CreateMap<UpdateTournamentDto, Tournament>();
    }
}

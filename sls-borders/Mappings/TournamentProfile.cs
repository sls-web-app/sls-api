using AutoMapper;
using sls_borders.DTO.TournamentDto;
using sls_borders.Models;
using System;

namespace sls_borders.Mappings
{
    public class TournamentProfile : Profile
    {
        public TournamentProfile()
        {
            // Maps from the data transfer object for creation to the main entity.
            // We ignore navigation properties, as the repository should handle loading them.
            CreateMap<CreateTournamentDto, Tournament>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.TournamentStatus.Upcoming))
                .ForMember(dest => dest.Round, opt => opt.Ignore()) // Should be handled by tournament logic
                .ForMember(dest => dest.OrganizingTeam, opt => opt.Ignore())
                .ForMember(dest => dest.Teams, opt => opt.Ignore())
                .ForMember(dest => dest.Games, opt => opt.Ignore());

            // Maps from the data transfer object for updates to the main entity.
            CreateMap<UpdateTournamentDto, Tournament>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Never map the ID on an update
                .ForMember(dest => dest.OrganizingTeam, opt => opt.Ignore())
                .ForMember(dest => dest.Teams, opt => opt.Ignore())
                .ForMember(dest => dest.Games, opt => opt.Ignore());

            // Maps from the main entity to the data transfer object for retrieval.
            // This will automatically handle mapping the collections of Teams and Games.
            CreateMap<Tournament, GetTournamentDto>();
        }
    }
}
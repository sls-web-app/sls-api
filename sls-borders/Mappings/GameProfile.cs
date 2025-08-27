using AutoMapper;
using sls_borders.DTO.GameDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for Game-related mappings.
/// </summary>
public class GameProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameProfile"/> class.
    /// </summary>
    public GameProfile()
    {
        // Game -> GetGameDto
        CreateMap<Game, GetGameDto>();

        // CreateGameDto -> Game
        CreateMap<CreateGameDto, Game>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Round, opt => opt.Ignore())
            .ForMember(dest => dest.Score, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentId, opt => opt.MapFrom(src => src.TournamentId))
            .ForMember(dest => dest.WhitePlayerId, opt => opt.MapFrom(src => src.WhitePlayerId))
            .ForMember(dest => dest.BlackPlayerId, opt => opt.MapFrom(src => src.BlackPlayerId));

        // UpdateGameDto -> Game
        CreateMap<UpdateGameDto, Game>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Round, opt => opt.MapFrom(src => src.Round))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
            .ForMember(dest => dest.TournamentId, opt => opt.MapFrom(src => src.TournamentId))
            .ForMember(dest => dest.WhitePlayerId, opt => opt.MapFrom(src => src.WhitePlayerId))
            .ForMember(dest => dest.BlackPlayerId, opt => opt.MapFrom(src => src.BlackPlayerId));
    }
}
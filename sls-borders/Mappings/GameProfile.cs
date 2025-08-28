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
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
    }
}
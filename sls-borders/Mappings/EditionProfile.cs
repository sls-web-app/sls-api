using AutoMapper;
using sls_borders.DTO.EditionDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for Edition-related mappings.
/// </summary>
public class EditionProfile : Profile
{
    public EditionProfile()
    {
        CreateMap<Edition, GetEditionDto>();

        CreateMap<CreateEditionDto, Edition>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}

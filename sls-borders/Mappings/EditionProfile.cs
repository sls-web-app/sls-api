using AutoMapper;
using sls_borders.DTO.EditionDto;
using sls_borders.Models;

namespace sls_borders.Mappings
{
    public class EditionProfile : Profile
    {
        public EditionProfile()
        {
            CreateMap<Edition, GetEditionDto>();

            CreateMap<CreateEditionDto, Edition>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}

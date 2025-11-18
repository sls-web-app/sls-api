using AutoMapper;
using sls_borders.DTO.ImageDto;
using sls_borders.Models;

namespace sls_borders.Mappings
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            // Map from Image entity to GetImageDto
            CreateMap<Image, GetImageDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));

            // Map from CreateImageDto to Image entity
            CreateMap<CreateImageDto, Image>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Tournament, opt => opt.Ignore())
                .ForMember(dest => dest.Edition, opt => opt.Ignore());
        }
    }
}

using AutoMapper;
using sls_borders.DTO.ImageDto;
using sls_borders.Models;

namespace sls_borders.Mappings
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Image, GetImageDto>();
            CreateMap<CreateImageDto, Image>();
        }
    }
}

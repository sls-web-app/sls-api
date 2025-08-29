using AutoMapper;
using sls_borders.DTO.AdminDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for Admin-related mappings.
/// </summary>
public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<CreateAdminDto, Admin>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

        CreateMap<Admin, GetAdminDto>();

        CreateMap<UpdateAdminDto, Admin>();
    }
}

using AutoMapper;
using sls_borders.DTO.AdminDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for Admin-related mappings.
/// </summary>
public class AdminProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProfile"/> class.
    /// </summary>
    public AdminProfile()
    {
        CreateMap<CreateAdminDto, Admin>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());

        CreateMap<UpdateAdminDto, Admin>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());

        CreateMap<Admin, GetAdminDto>();
    }
}

using AutoMapper;
using sls_borders.DTO.UserDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

/// <summary>
/// AutoMapper profile for User-related mappings.
/// </summary>
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

        CreateMap<User, GetUserDto>();

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AccountActivated, opt => opt.Ignore())
            .ForMember(dest => dest.IsInPlay, opt => opt.Ignore());
    }
}


using AutoMapper;
using sls_borders.DTO.UserDto;
using sls_borders.Models;

namespace sls_borders.Mappings;

public class UserInviteProfile : Profile
{
    public UserInviteProfile()
    {
        // InviteUserDto -> UserInvite
        CreateMap<InviteUserDto, UserInvite>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }
}
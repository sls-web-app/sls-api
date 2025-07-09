using AutoMapper;
using sls_borders.DTO.UserDto;
using sls_borders.Models;
using System.Security.Cryptography;
using System.Text;

namespace sls_borders.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsWhite, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsBlack, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsWhite, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsBlack, opt => opt.Ignore());

            CreateMap<User, GetUserDto>();
        }
    }
}
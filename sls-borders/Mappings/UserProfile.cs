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
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password).Hash))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => HashPassword(src.Password).Salt))
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsWhite, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsBlack, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Condition(src => src.Password != null))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => 
                    src.Password != null ? HashPassword(src.Password).Hash : string.Empty))
                .ForMember(dest => dest.PasswordSalt, opt => opt.Condition(src => src.Password != null))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => 
                    src.Password != null ? HashPassword(src.Password).Salt : string.Empty))
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsWhite, opt => opt.Ignore())
                .ForMember(dest => dest.GamesAsBlack, opt => opt.Ignore());

            CreateMap<User, GetUserDto>();
        }

        private (string Hash, string Salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var saltBytes = hmac.Key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = hmac.ComputeHash(passwordBytes);
            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }
    }
}
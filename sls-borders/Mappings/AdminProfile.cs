using AutoMapper;
using sls_borders.DTO.Admin;
using sls_borders.Models;
using System.Security.Cryptography;
using System.Text;

namespace sls_borders.Mappings
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<CreateAdminDto, Admin>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password).Hash))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => HashPassword(src.Password).Salt));

            CreateMap<UpdateAdminDto, Admin>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.Condition(src => src.Password != null))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => 
                    src.Password != null ? HashPassword(src.Password).Hash : string.Empty))
                .ForMember(dest => dest.PasswordSalt, opt => opt.Condition(src => src.Password != null))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => 
                    src.Password != null ? HashPassword(src.Password).Salt : string.Empty));
                
            CreateMap<Admin, GetAdminDto>();
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
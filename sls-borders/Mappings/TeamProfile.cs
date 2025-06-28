using AutoMapper;
using sls_borders.DTO.Team;
using sls_borders.Models;
using sls_borders.Data;

namespace sls_borders.Mappings
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            // Team -> GetTeamDto
            CreateMap<Team, GetTeamDto>();

            // CreateTeamDto -> Team
            CreateMap<CreateTeamDto, Team>()
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Tournaments, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizedTournaments, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    var dbContext = context.Items["DbContext"] as ApplicationDbContext;
                    if (dbContext == null) return;

                    // Map Users from UsersId
                    if (src.UsersId?.Any() == true)
                    {
                        dest.Users = dbContext.Users
                            .Where(u => src.UsersId.Contains(u.Id))
                            .ToList();
                    }

                    // Map Tournaments from TournamentsId
                    if (src.TournamentsId?.Any() == true)
                    {
                        dest.Tournaments = dbContext.Tournaments
                            .Where(t => src.TournamentsId.Contains(t.Id))
                            .ToList();
                    }
                });

            // UpdateTeamDto -> Team
            CreateMap<UpdateTeamDto, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Tournaments, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizedTournaments, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    var dbContext = context.Items["DbContext"] as ApplicationDbContext;
                    if (dbContext == null) return;

                    // Update Users from UsersId
                    if (src.UsersId?.Any() == true)
                    {
                        dest.Users = dbContext.Users
                            .Where(u => src.UsersId.Contains(u.Id))
                            .ToList();
                    }
                    else
                    {
                        dest.Users.Clear();
                    }

                    // Update Tournaments from TournamentsId
                    if (src.TournamentsId?.Any() == true)
                    {
                        dest.Tournaments = dbContext.Tournaments
                            .Where(t => src.TournamentsId.Contains(t.Id))
                            .ToList();
                    }
                    else
                    {
                        dest.Tournaments.Clear();
                    }
                });
        }
    }
}
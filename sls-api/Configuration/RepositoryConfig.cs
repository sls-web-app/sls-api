using sls_borders.Repositories;
using sls_repos.Repositories;

namespace sls_api.Configuration
{
    public static class RepositoryConfig
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAdminRepo, AdminRepo>();
            services.AddScoped<IGameRepo, GameRepo>();
            services.AddScoped<ITeamRepo, TeamRepo>();
            services.AddScoped<ITournamentRepo, TournamentRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserInviteRepo, UserInviteRepo>();
            services.AddScoped<IEditionRepo, EditionRepo>();
            services.AddScoped<IImageRepo, ImageRepo>();

            return services;
        }
    }
}
using sls_borders.Models;
using Microsoft.EntityFrameworkCore;

namespace sls_repos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Tournament> Tournaments { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Username).IsRequired().HasMaxLength(50);
                entity.Property(a => a.PasswordHash).IsRequired();
                entity.Property(a => a.PasswordSalt).IsRequired();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Adress).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Img).IsRequired().HasMaxLength(200);

                //One-to-many relationship with Users
                entity.HasMany(t => t.Users)
                    .WithOne(u => u.Team)
                    .HasForeignKey(u => u.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                
                //One-to-many relationship with Tournaments (organized tournaments)
                entity.HasMany(t => t.OrganizedTournaments)
                    .WithOne(t => t.OrganizingTeam)
                    .HasForeignKey(t => t.OrganizingTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                //Many-to-many relationship with Tournaments (playing tournaments)
                entity.HasMany(t => t.Tournaments)
                    .WithMany(t => t.Teams)
                    .UsingEntity(j => j.ToTable("TeamTournaments"));
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();
                entity.Property(u => u.ProfileImg).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Surname).IsRequired().HasMaxLength(50);
                entity.Property(u => u.ClassName).HasMaxLength(50);

                //One-to-many relationship with Team
                entity.HasOne(u => u.Team)
                    .WithMany(t => t.Users)
                    .HasForeignKey(u => u.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                //One-to-many relationship with Games (as White Player)
                entity.HasMany(u => u.GamesAsWhite)
                    .WithOne(g => g.WhitePlayer)
                    .HasForeignKey(g => g.WhitePlayerId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                //One-to-many relationship with Games (as Black Player)
                entity.HasMany(u => u.GamesAsBlack)
                    .WithOne(g => g.BlackPlayer)
                    .HasForeignKey(g => g.BlackPlayerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Date).IsRequired();
                entity.Property(t => t.Round).IsRequired(false);
                entity.Property(t => t.Status).IsRequired();

                //One-to-many relationship with Teams (organizing team)
                entity.HasOne(t => t.OrganizingTeam)
                    .WithMany(t => t.OrganizedTournaments)
                    .HasForeignKey(t => t.OrganizingTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                //Many-to-many relationship with Teams (playing teams)
                entity.HasMany(t => t.Teams)
                    .WithMany(t => t.Tournaments)
                    .UsingEntity(j => j.ToTable("TeamTournaments"));

                //One-to-many relationship with Games
                entity.HasMany(t => t.Games)
                    .WithOne(g => g.Tournament)
                    .HasForeignKey(g => g.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Round).IsRequired();
                entity.Property(g => g.Score).IsRequired(false);

                //One-to-many relationship with Tournament
                entity.HasOne(g => g.Tournament)
                    .WithMany(t => t.Games)
                    .HasForeignKey(g => g.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

                //One-to-many relationship with Users (as White Player)
                entity.HasOne(g => g.WhitePlayer)
                    .WithMany(u => u.GamesAsWhite)
                    .HasForeignKey(g => g.WhitePlayerId)
                    .OnDelete(DeleteBehavior.SetNull);

                //One-to-many relationship with Users (as Black Player)
                entity.HasOne(g => g.BlackPlayer)
                    .WithMany(u => u.GamesAsBlack)
                    .HasForeignKey(g => g.BlackPlayerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
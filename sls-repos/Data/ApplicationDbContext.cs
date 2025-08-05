using sls_borders.Models;
using Microsoft.EntityFrameworkCore;

namespace sls_borders.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Tournament> Tournaments { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Edition> Editions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Username).IsRequired().HasMaxLength(50);
                entity.Property(a => a.PasswordHash).IsRequired();
                entity.Property(a => a.PasswordSalt).IsRequired();

                //Make sure Username is unique
                entity.HasIndex(a => a.Username).IsUnique();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Short).IsRequired().HasMaxLength(20);
                entity.Property(t => t.Address).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Img).HasMaxLength(200);
                entity.Property(t => t.CreatedAt).HasConversion(
                    v => v,
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

                //One-to-many relationship with Users
                entity.HasMany(t => t.Users)
                    .WithOne(u => u.Team)
                    .HasForeignKey(u => u.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(t => t.OrganizedTournaments)
                    .WithOne(t => t.OrganizingTeam)
                    .HasForeignKey(t => t.OrganizingTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

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
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Date).IsRequired();
                entity.Property(t => t.Round).IsRequired(false);
                entity.Property(t => t.Status).IsRequired();
                
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
                
                entity.HasOne<User>()
                    .WithMany(u => u.GamesAsWhite)
                    .HasForeignKey(g => g.WhitePlayerId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne<User>()
                    .WithMany(u => u.GamesAsBlack)
                    .HasForeignKey(g => g.BlackPlayerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Edition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.StartDate)
                    .HasColumnType("date");
                entity.Property(e => e.EndDate)
                    .HasColumnType("date");
                entity.Property(e => e.Organizer).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasConversion(
                    v => v,
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

                //One-to-many relationship with Teams
                entity.HasMany(e => e.Teams)
                    .WithOne(t => t.Edition)
                    .HasForeignKey(t => t.EditionId)
                    .OnDelete(DeleteBehavior.SetNull);
                //One-to-many relationship with Tournaments
                entity.HasMany(e => e.Tournaments)
                    .WithOne(t => t.Edition)
                    .HasForeignKey(t => t.EditionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
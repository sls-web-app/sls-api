using sls_borders.Models;
using Microsoft.EntityFrameworkCore;

namespace sls_borders.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Team> Teams { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Tournament> Tournaments { get; set; } = default!;
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<UserInvite> UserInvites { get; set; } = default!;
        public DbSet<Edition> Editions { get; set; } = default!;
        public DbSet<EditionTeamMember> EditionTeamMembers { get; set; } = default!;

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

            modelBuilder.Entity<UserInvite>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);

                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Edition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(7) // Hex color code length
                    .HasConversion(
                        v => v,
                        v => v.StartsWith("#") ? v : "#" + v // Ensure it starts with '#'
                    );
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.StartDate)
                    .HasColumnType("date");
                entity.Property(e => e.EndDate)
                    .HasColumnType("date");
                entity.Property(e => e.Organizer).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasConversion(
                    v => v,
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

                //One-to-many relationship with Tournaments
                entity.HasMany(e => e.Tournaments)
                    .WithOne(t => t.Edition)
                    .HasForeignKey(t => t.EditionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // EditionTeamMember
            modelBuilder.Entity<EditionTeamMember>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Edition)
                      .WithMany(e => e.EditionTeamMembers)
                      .HasForeignKey(e => e.EditionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Team)
                      .WithMany(t => t.EditionTeamMembers)
                      .HasForeignKey(e => e.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.EditionTeamMembers)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
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
        public DbSet<Image> Images { get; set; } = default!;

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

                //Many-to-many relationship with Editions
                entity.HasMany(t => t.Editions)
                    .WithMany(e => e.Teams)
                    .UsingEntity(j => j.ToTable("EditionTeams"));
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.ProfileImg).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Surname).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();

                //Many-to-one relationship with Team
                entity.HasOne(u => u.Team)
                    .WithMany(t => t.Users)
                    .HasForeignKey(u => u.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Date).IsRequired();
                entity.Property(t => t.Round).IsRequired(false);
                entity.Property(t => t.Status).IsRequired();

                //Many-to-one relationship with Edition
                entity.HasOne(t => t.Edition)
                    .WithMany(e => e.Tournaments)
                    .HasForeignKey(t => t.EditionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Round).IsRequired();
                entity.Property(g => g.Score).IsRequired(false);

                //Many-to-one relationship with Tournament
                entity.HasOne(g => g.Tournament)
                    .WithMany(t => t.Games)
                    .HasForeignKey(g => g.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

                //Many-to-one relationship with White Player (User)
                entity.HasOne(g => g.WhitePlayer)
                    .WithMany(u => u.GamesAsWhite)
                    .HasForeignKey(g => g.WhitePlayerId)
                    .OnDelete(DeleteBehavior.SetNull);

                //Many-to-one relationship with Black Player (User)
                entity.HasOne(g => g.BlackPlayer)
                    .WithMany(u => u.GamesAsBlack)
                    .HasForeignKey(g => g.BlackPlayerId)
                    .OnDelete(DeleteBehavior.SetNull);

                //Many-to-one relationship with White Team
                entity.HasOne(g => g.WhiteTeam)
                    .WithMany(t => t.GamesAsWhite)
                    .HasForeignKey(g => g.WhiteTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                //Many-to-one relationship with Black Team
                entity.HasOne(g => g.BlackTeam)
                    .WithMany(t => t.GamesAsBlack)
                    .HasForeignKey(g => g.BlackTeamId)
                    .OnDelete(DeleteBehavior.SetNull);
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
            });

            modelBuilder.Entity<UserInvite>(entity =>
            {
                entity.HasKey(ui => ui.Id);
                entity.Property(ui => ui.UserId).IsRequired();

                //One-to-one relationship with User
                entity.HasOne(ui => ui.User)
                    .WithOne(u => u.Invite)
                    .HasForeignKey<UserInvite>(ui => ui.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Title).IsRequired().HasMaxLength(200);
                entity.Property(i => i.FileName).IsRequired().HasMaxLength(256);
                entity.Property(i => i.ContentType).IsRequired();
                entity.Property(i => i.UploadedAt).HasConversion(
                    v => v,
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
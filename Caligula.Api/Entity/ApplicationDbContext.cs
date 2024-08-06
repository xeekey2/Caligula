using Microsoft.EntityFrameworkCore;
using Caligula.Model.DBModels;

namespace Caligula.Service.Entity
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DbMatch> Matches { get; set; }
        public DbSet<DbParticipant> Participants { get; set; }
        public DbSet<DbPlayer> Players { get; set; }
        public DbSet<DbMap> Maps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DbPlayer>()
                .HasKey(p => p.ProPlayerId);

            modelBuilder.Entity<DbParticipant>()
                .HasOne(p => p.DbMatch)
                .WithMany(m => m.Participants)
                .HasForeignKey(p => p.DbMatchId);

            modelBuilder.Entity<DbParticipant>()
                .HasOne(p => p.DbPlayer)
                .WithMany(pl => pl.Participants)
                .HasForeignKey(p => p.ProPlayerId);

            modelBuilder.Entity<DbMatch>()
                .HasOne(m => m.Map)
                .WithMany(mp => mp.Matches)
                .HasForeignKey(m => m.MapId);
        }
    }
}

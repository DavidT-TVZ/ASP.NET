using DnD_Character_Sheet_Creator.Models;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Data
{
    public class DnDDbContext : DbContext
    {
        public DnDDbContext(DbContextOptions<DnDDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterLevel> CharacterLevels { get; set; }
        public DbSet<Equipment> Equipment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Surname).IsRequired();
                entity.Property(p => p.Username).IsRequired();
                entity.Property(p => p.Email).IsRequired();
                entity.Property(p => p.Password).IsRequired();

                entity.HasMany(p => p.CharacterList)
                    .WithOne(c => c.Player)
                    .HasForeignKey(c => c.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(c => c.CharacterId);
                entity.Property(c => c.CharacterName).IsRequired();
                entity.Property(c => c.PlayerId).IsRequired();

                entity.HasOne(c => c.Level)
                    .WithOne()
                    .HasForeignKey<Character>(c => c.LevelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.EquipmentList)
                    .WithOne(e => e.Character)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CharacterLevel>(entity =>
            {
                entity.HasKey(cl => cl.LevelId);
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentId);
                entity.Property(e => e.Name).IsRequired();
            });
        }
    }
}

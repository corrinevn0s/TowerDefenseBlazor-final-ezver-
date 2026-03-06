using Microsoft.EntityFrameworkCore;
using TowerDefenseBlazor.API.Models;

namespace TowerDefenseBlazor.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LeaderboardEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MaxWave).IsRequired();
            entity.HasIndex(e => e.MaxWave).IsDescending(true);
        });
    }
}
using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Data;

public class GridlyDbContext(DbContextOptions<GridlyDbContext> options) : DbContext(options)
{
    public DbSet<CardEntity> Cards => Set<CardEntity>();
    public DbSet<SettingsEntity> Settings => Set<SettingsEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardEntity>(entity =>
        {
            entity.ToTable("Card");
            entity.HasKey(c => c.Id);
            entity.HasOne(c => c.Settings)
                .WithOne()
                .HasForeignKey<SettingsEntity>(s => s.CardId);
        });

        modelBuilder.Entity<SettingsEntity>(entity =>
        {
            entity.ToTable("Settings");
            entity.HasKey(s => s.Id);
        });
    }
}

using Gridly.Models;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Data;

public class GridlyDbContext(DbContextOptions<GridlyDbContext> options) : DbContext(options)
{
    public DbSet<SettingsModel> Cards => Set<SettingsModel>();
    public DbSet<SettingsModel> Settings => Set<SettingsModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardModel>(entity =>
        {
            entity.ToTable("Card");
            entity.HasKey(c => c.Id);
            entity.HasOne(c => c.Settings)
                .WithOne()
                .HasForeignKey<SettingsModel>(s => s.CardId);
        });

        modelBuilder.Entity<SettingsModel>(entity =>
        {
            entity.ToTable("Settings");
            entity.HasKey(s => s.Id);
        });
    }
}

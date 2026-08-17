using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Data;

public class GridlyDbContext(DbContextOptions<GridlyDbContext> options) : DbContext(options), IGridlyDbContext
{
    public DbSet<CardEntity> Cards => Set<CardEntity>();
    public DbSet<SettingsEntity> Settings => Set<SettingsEntity>();
    public DbSet<IconsConnectedEntity> IconsConnected => Set<IconsConnectedEntity>();
    public DbSet<IconEntity> Icons => Set<IconEntity>();

    IQueryable<CardEntity> IGridlyDbContext.Cards => Cards;
    IQueryable<SettingsEntity> IGridlyDbContext.Settings => Settings;
    IQueryable<IconsConnectedEntity> IGridlyDbContext.IconsConnected => IconsConnected;
    IQueryable<IconEntity> IGridlyDbContext.Icons => Icons;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardEntity>(e =>
        {
            e.ToTable("Card");
            e.HasKey(c => c.Id);
            e.Property(c => c.Url).HasColumnName("URL");
            e.HasMany(c => c.Settings).WithOne(s => s.Card)
                .HasForeignKey(s => s.CardId).OnDelete(DeleteBehavior.NoAction);
            e.HasMany(c => c.IconsConnected).WithOne(ic => ic.Card)
                .HasForeignKey(ic => ic.CardId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SettingsEntity>(e =>
        {
            e.ToTable("Settings");
            e.HasKey(s => s.Id);
        });

        modelBuilder.Entity<IconsConnectedEntity>(e =>
        {
            e.ToTable("IconsConnected");
            e.HasKey(ic => ic.Id);
            e.HasOne(ic => ic.Icon).WithMany(i => i.IconsConnected)
                .HasForeignKey(ic => ic.IconId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<IconEntity>(e =>
        {
            e.ToTable("Icon");
            e.HasKey(i => i.Id);
        });
    }
}

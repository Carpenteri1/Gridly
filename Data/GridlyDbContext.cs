using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Data;

public class GridlyDbContext(DbContextOptions<GridlyDbContext> options) : DbContext(options)
{
    public DbSet<CardEntity> Cards => Set<CardEntity>();
    public DbSet<SettingsEntity> Settings => Set<SettingsEntity>();
    public DbSet<IconsConnectedEntity> IconsConnected => Set<IconsConnectedEntity>();
    public DbSet<IconEntity> Icons => Set<IconEntity>();
    public DbSet<RowColumnEntity> RowColumns => Set<RowColumnEntity>();
    public DbSet<WidgetEntity> Widgets => Set<WidgetEntity>();
    public DbSet<WidgetTypeEntity> WidgetTypes => Set<WidgetTypeEntity>();
    public DbSet<WeatherDataConnectionEntity> WeatherDataConnections => Set<WeatherDataConnectionEntity>();
    public DbSet<ProviderKeyEntity> ProviderKeys => Set<ProviderKeyEntity>();
    public DbSet<WeatherDataEntity> WeatherData => Set<WeatherDataEntity>();

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

        modelBuilder.Entity<RowColumnEntity>(e =>
        {
            e.ToTable("RowColumn");
            e.HasKey(r => r.Id);
        });

        modelBuilder.Entity<WidgetEntity>(e =>
        {
            e.ToTable("Widget");
            e.HasKey(w => w.Id);
        });

        modelBuilder.Entity<WidgetTypeEntity>(e =>
        {
            e.ToTable("WidgetType");
            e.HasKey(wt => wt.Id);
        });

        modelBuilder.Entity<WeatherDataConnectionEntity>(e =>
        {
            e.ToTable("WeatherDataConnection");
            e.HasKey(w => w.Id);
            e.HasIndex(w => w.CardId).IsUnique();
        });
        
        modelBuilder.Entity<WeatherDataEntity>(e =>
        {
            e.ToTable("WeatherData");
            e.HasKey(w => w.Id);
            e.HasIndex(w => w.Address).IsUnique();
        });

        modelBuilder.Entity<ProviderKeyEntity>(e =>
        {
            e.ToTable("ProviderKeys");
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Provider).IsUnique();
        });
    }
}

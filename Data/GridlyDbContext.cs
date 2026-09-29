using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Data;

public class GridlyDbContext(DbContextOptions<GridlyDbContext> options) : DbContext(options)
{
    public DbSet<CardEntity> Cards => Set<CardEntity>();
    public DbSet<SettingsEntity> Settings => Set<SettingsEntity>();
    public DbSet<IconsConnectedEntity> IconsConnected => Set<IconsConnectedEntity>();
    public DbSet<IconEntity> Icons => Set<IconEntity>();
    public DbSet<ColumnRowEntity> RowColumns => Set<ColumnRowEntity>();
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
            e.HasOne(c => c.Settings).WithOne(x => x.Card)
                .HasForeignKey<SettingsEntity>(s => s.CardId)
                .OnDelete(DeleteBehavior.Cascade);
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
                .HasForeignKey(ic => ic.IconId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IconEntity>(e =>
        {
            e.ToTable("Icon");
            e.HasKey(i => i.Id);
        });

        modelBuilder.Entity<ColumnRowEntity>(e =>
        {
            e.ToTable("RowColumn");
            e.HasKey(r => r.Id);
            e.HasMany(c => c.Cards)
                .WithOne()
                .HasForeignKey(c => c.RowColumnId);
        });

        modelBuilder.Entity<WidgetEntity>(e =>
        {
            e.ToTable("Widget");
            e.HasKey(w => w.Id);
            e.HasData(
                new WidgetEntity { Id = 1, Description = "", Icon = "cloud", Label = "Weather", WidgetType = 3 },
                new WidgetEntity { Id = 2, Description = "", Icon = "box", Label = "Custom", WidgetType = 2 },
                new WidgetEntity { Id = 3, Description = "", Icon = "Box", Label = "Empty", WidgetType = 1 }
            );
        });

        modelBuilder.Entity<WidgetTypeEntity>(e =>
        {
            e.ToTable("WidgetType");
            e.HasKey(wt => wt.Id);
            e.HasData(
                new WidgetTypeEntity { Id = 1, Name = "Empty"},
                new WidgetTypeEntity { Id = 2, Name = "Custom"}, 
                new WidgetTypeEntity { Id = 3, Name = "Weather"});
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
            e.HasData(
                new WeatherDataEntity { Id = 1, Address = "London", Description = "Cloudy", Temp = 10, FeelsLike = 11, Humidity = 50, WindSpeed = 5, WindDir = 180, FetchedAt = DateTime.UtcNow, Timezone = "Europe/London"},
                new WeatherDataEntity { Id = 2, Address = "Stockholm", Description = "Clear", Temp = 20, FeelsLike = 20, Humidity = 50, WindSpeed = 5, WindDir = 180, FetchedAt = DateTime.UtcNow, Timezone = "Europe/Stockholm"}
            );
        });

        modelBuilder.Entity<ProviderKeyEntity>(e =>
        {
            e.ToTable("ProviderKeys");
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Provider).IsUnique();
        });
    }
}

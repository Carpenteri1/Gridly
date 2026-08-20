using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class IconRepositoryTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"gridly-icons-{Guid.NewGuid():N}");

    [Fact]
    public void FindUnusedIcons_ReturnsOnlyFilesNotReferencedByCards()
    {
        Directory.CreateDirectory(_tempDirectory);
        var usedIcon = CreateFile("used.svg");
        var unusedIcon = CreateFile("unused.png");
        var fileService = new FakeFileService
        {
            Icons = new[] { new FileInfo(usedIcon), new FileInfo(unusedIcon) }
        };
        var repository = new IconRepository(null!, null!, fileService);
        var cards = new[]
        {
            new CardModel
            {
                IconData = new IconModel { Name = "used", Type = "svg", Base64Data = "", MaterialIcon = "" }
            }
        };

        var result = repository.FindUnusedIcons(cards);

        Assert.Single(result);
        Assert.Equal("unused.png", result[0]);
    }

    [Fact]
    public void FindUnusedIcons_WhenAllIconsAreUsed_ReturnsEmptyList()
    {
        Directory.CreateDirectory(_tempDirectory);
        var usedIcon = CreateFile("used.svg");
        var fileService = new FakeFileService
        {
            Icons = new[] { new FileInfo(usedIcon) }
        };
        var repository = new IconRepository(null!, null!, fileService);
        var cards = new[]
        {
            new CardModel
            {
                IconData = new IconModel { Name = "used", Type = "svg", Base64Data = "", MaterialIcon = "" }
            }
        };

        var result = repository.FindUnusedIcons(cards);

        Assert.Empty(result);
    }

    [Fact]
    public void FindUnusedIcons_WhenThereAreNoFiles_ReturnsEmptyList()
    {
        var repository = new IconRepository(null!, null!, new FakeFileService());

        var result = repository.FindUnusedIcons(Array.Empty<CardModel>());

        Assert.Empty(result);
    }

    private string CreateFile(string fileName)
    {
        var filePath = Path.Combine(_tempDirectory, fileName);
        File.WriteAllText(filePath, fileName);
        return filePath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}

public sealed class IconRepositoryInsertTests
{
    [Fact]
    public async Task Insert_WhenIconIsValid_PersistsRowAndReturnsIconWithGeneratedId()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new IconRepository(connection, dbContext, new FakeFileService());

        var icon = new IconModel
        {
            Name = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = "dashboard",
        };

        var result = await repository.Insert(icon);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("grid", result.Name);
        Assert.Equal("svg", result.Type);
        Assert.Equal("Zm9v", result.Base64Data);
        Assert.Equal("dashboard", result.MaterialIcon);

        var persisted = await dbContext.Icons.SingleAsync(i => i.Id == result.Id);
        Assert.Equal("grid", persisted.Name);
        Assert.Equal("svg", persisted.Type);
        Assert.Equal("Zm9v", persisted.Base64Data);
        Assert.Equal("dashboard", persisted.MaterialIcon);
    }
}

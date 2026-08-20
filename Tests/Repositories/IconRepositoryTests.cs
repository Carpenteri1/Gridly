using Gridly.Data;
using Gridly.Entities;
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
        var repository = new IconRepository(null!, fileService, null!);
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
        var repository = new IconRepository(null!, fileService, null!);
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
        var repository = new IconRepository(null!, new FakeFileService(), null!);

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

public sealed class IconRepositoryDeleteTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly IconRepository _repository;

    public IconRepositoryDeleteTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new IconRepository(_connection, new FakeFileService(), _dbContext);
    }

    [Fact]
    public async Task Delete_WhenIconExists_RemovesRowAndReturnsTrue()
    {
        var toDelete = new IconEntity { Name = "old", Type = "svg", Base64Data = "abc", MaterialIcon = "box" };
        var toKeep = new IconEntity { Name = "keep", Type = "png", Base64Data = "def", MaterialIcon = "star" };
        _dbContext.Icons.AddRange(toDelete, toKeep);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Delete(toDelete.Id);

        Assert.True(result);
        var remaining = await _dbContext.Icons.AsNoTracking().ToListAsync();
        Assert.Single(remaining);
        Assert.Equal(toKeep.Id, remaining[0].Id);
    }

    [Fact]
    public async Task Delete_WhenIconDoesNotExist_ReturnsFalse()
    {
        var existing = new IconEntity { Name = "keep", Type = "png", Base64Data = "def", MaterialIcon = "star" };
        _dbContext.Icons.Add(existing);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Delete(existing.Id + 1);

        Assert.False(result);
        Assert.Single(await _dbContext.Icons.AsNoTracking().ToListAsync());
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}

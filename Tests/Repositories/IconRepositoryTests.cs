using Gridly.Models;
using Gridly.Repositories;
using Gridly.Tests.Infrastructure;

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
        var repository = new IconRepository(null!, fileService);
        var cards = new[]
        {
            new CardModel
            {
                IconData = new IconModel { Name = "used", Type = "svg" }
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
        var repository = new IconRepository(null!, fileService);
        var cards = new[]
        {
            new CardModel
            {
                IconData = new IconModel { Name = "used", Type = "svg" }
            }
        };

        var result = repository.FindUnusedIcons(cards);

        Assert.Empty(result);
    }

    [Fact]
    public void FindUnusedIcons_WhenThereAreNoFiles_ReturnsEmptyList()
    {
        var repository = new IconRepository(null!, new FakeFileService());

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

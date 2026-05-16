using Gridly.Models;
using Gridly.Services;

namespace Gridly.Tests.Services;

public sealed class FileServiceTests : IDisposable
{
    private readonly FileService _service = new();
    private readonly string _iconDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Icons");
    private readonly List<string> _createdFiles = new();

    [Fact]
    public async Task WriteToFile_WritesExpectedText()
    {
        var filePath = CreateUniquePath(".txt");

        var result = _service.WriteToFile(filePath, "hello world");

        Assert.True(result);
        Assert.Equal("hello world", await File.ReadAllTextAsync(filePath));
    }

    [Fact]
    public void WriteAllBitesToFile_WritesDecodedBytes()
    {
        var filePath = CreateUniquePath(".bin");
        var base64 = Convert.ToBase64String([1, 2, 3, 4]);

        var result = _service.WriteAllBitesToFile(filePath, base64);

        Assert.True(result);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, File.ReadAllBytes(filePath));
    }

    [Fact]
    public void DeletedFile_RemovesExistingFile()
    {
        var filePath = CreateUniquePath(".txt");
        File.WriteAllText(filePath, "content");

        var result = _service.DeletedFile(filePath);

        Assert.True(result);
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public void UploadIcon_CreatesFileInIconDirectory()
    {
        EnsureIconDirectory();
        var fileName = $"upload-{Guid.NewGuid():N}";
        var icon = new IconModel
        {
            Name = fileName,
            Type = "png",
            Base64Data = Convert.ToBase64String([5, 6, 7])
        };

        var result = _service.UploadIcon(icon);
        var filePath = Path.Combine(_iconDirectory, $"{fileName}.png");
        _createdFiles.Add(filePath);

        Assert.True(result);
        Assert.True(File.Exists(filePath));
        Assert.Equal(new byte[] { 5, 6, 7 }, File.ReadAllBytes(filePath));
    }

    [Fact]
    public void DeleteIcon_RemovesFileFromIconDirectory()
    {
        EnsureIconDirectory();
        var fileName = $"delete-{Guid.NewGuid():N}";
        var filePath = Path.Combine(_iconDirectory, $"{fileName}.svg");
        File.WriteAllText(filePath, "svg");

        var result = _service.DeleteIcon(fileName, "svg");

        Assert.True(result);
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public void GetAllIcons_ReturnsAllowedFilesAndExcludesFavicon()
    {
        EnsureIconDirectory();
        var includedPath = Path.Combine(_iconDirectory, $"keep-{Guid.NewGuid():N}.png");
        var excludedPath = Path.Combine(_iconDirectory, $"skip-{Guid.NewGuid():N}.txt");
        var faviconPath = Path.Combine(_iconDirectory, "favicon.ico");

        File.WriteAllText(includedPath, "png");
        File.WriteAllText(excludedPath, "txt");
        var restoreFavicon = !File.Exists(faviconPath);
        if (restoreFavicon)
        {
            File.WriteAllText(faviconPath, "ico");
        }

        _createdFiles.Add(includedPath);
        _createdFiles.Add(excludedPath);
        if (restoreFavicon)
        {
            _createdFiles.Add(faviconPath);
        }

        var result = _service.GetAllIcons().Select(file => file.Name).ToArray();

        Assert.Contains(Path.GetFileName(includedPath), result);
        Assert.DoesNotContain(Path.GetFileName(excludedPath), result);
        Assert.DoesNotContain("favicon.ico", result);
    }

    private string CreateUniquePath(string extension)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"gridly-{Guid.NewGuid():N}{extension}");
        _createdFiles.Add(filePath);
        return filePath;
    }

    private void EnsureIconDirectory()
    {
        Directory.CreateDirectory(_iconDirectory);
    }

    public void Dispose()
    {
        foreach (var filePath in _createdFiles)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

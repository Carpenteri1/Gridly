namespace Gridly.Services;

public interface IFileService
{
    public bool FileExist(string filePath);
    public bool DeletedFile(string filePath);
    public bool WriteAllBitesToFile(string filePath, string content);
    public bool WriteToFile(string filePath, string content);
    public Task<string> ReadAllFromFileAsync(string filePath);
    public IEnumerable<FileInfo> GetAllIcons();
}
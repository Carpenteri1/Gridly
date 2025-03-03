namespace Gridly.Services;

public interface IFileService
{
    public bool FileExcist(string filePath);
    public bool DeletedFile(string filePath);
    public bool WriteAllBites(string filePath, string content);
    public bool WriteToJson(string filePath, string content);
    public Task<string> ReadAllFromFileAsync(string filePath);
}
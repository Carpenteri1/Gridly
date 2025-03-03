namespace Gridly.Services;

public class FileService : IFileService
{
    public bool FileExcist(string filePath) =>  File.Exists(filePath);

    public bool DeletedFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception e) { Console.WriteLine(e.Message); return false; }

        return true;
    }

    public bool WriteAllBites(string filePath, string base64Data)
    {
        try
        {
            File.WriteAllBytes(filePath, Convert.FromBase64String(base64Data));
        }
        catch (Exception e) { Console.WriteLine(e.Message); return false; }

        return true;
    }
    
    public bool WriteToJson(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content);    
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        
        return true;
    }

    public async Task<string> ReadAllFromFileAsync(string filePath) => await File.ReadAllTextAsync(filePath); 
}
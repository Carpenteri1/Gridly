using Gridly.Configuration;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository(IDataConverter<ComponentModel> dataConverter, IFileService fileService) : IComponentRepository
{
    public bool Save(IEnumerable<ComponentModel> component)
    {
        string jsonString = dataConverter.SerializerToJsonString(component);
        return fileService.WriteToFile(FilePaths.ComponentPath, jsonString);
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var jsonFileString = "[]";
        if (!fileService.FileExist(FilePaths.ComponentPath))
        {
            fileService.WriteToFile(FilePaths.ComponentPath,jsonFileString);
            return dataConverter.DeserializeJsonToArray(jsonFileString);
        }
        
        jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.ComponentPath);
        return dataConverter.DeserializeJsonToArray(jsonFileString);
    }
    
    public async Task<ComponentModel?> GetById(int Id)
    {
        string jsonString = await fileService.ReadAllFromFileAsync(FilePaths.ComponentPath);
        return dataConverter.DeserializeJsonToArray(jsonString)?.ToList().First(x => x.Id == Id);
    }

    public bool UploadIcon(IconModel iconData)
    {
        string filePath = FilePaths.IconPath + $"{iconData.name}.{iconData.type}";
        return fileService.WriteAllBitesToFile(filePath, iconData.base64Data);
    }
    
    public bool DeleteIcon(string name, string type)
    {
        string filePath = FilePaths.IconPath + $"{name}.{type}";
        return fileService.FileExist(filePath) && fileService.DeletedFile(filePath);
    }
    
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components)
    {
        var unusedIcons = new List<string>();
        var iconFiles = fileService.GetAllIcons();
        
        foreach (var icon in iconFiles)
        {
            if (!components.Any(x => x.IconData != null 
                && $"{x.IconData.name}.{x.IconData.type}" == icon.Name))
            {
                unusedIcons.Add(icon.Name);
            }
        }
        
        return unusedIcons.Any() ? unusedIcons : new List<string>();
    }
}
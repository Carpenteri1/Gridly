using Gridly.Configuration;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository(IDataConverter<ComponentModel> dataConverter, IFileService fileService) : IComponentRepository
{
    public bool Save(IEnumerable<ComponentModel> newComponent)
    {
        string jsonString = dataConverter.SerializerToJsonString(newComponent);
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
    
    public bool DeleteIcon(IconModel iconData)
    {
        string filePath = FilePaths.IconPath + $"{iconData.name}.{iconData.type}";
        return fileService.FileExist(filePath) && fileService.DeletedFile(filePath);
    }

    public bool IconDuplicate(IEnumerable<ComponentModel>componentModels, IconModel iconData) =>
        componentModels.Where(x => x.IconData != null &&
                                    x.IconData.name == iconData.name &&
                                    x.IconData.type == iconData.type).Any();
    
}
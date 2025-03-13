using Gridly.Configuration;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository(IDataConverter<ComponentModel> dataConverter, IFileService fileService) : IComponentRepository
{
    public bool ReadToJsonFile(List<ComponentModel> newComponent)
    {
        string jsonString = dataConverter.SerializerToJsonString(newComponent);
        return fileService.WriteToJson(FilePaths.ComponentPath, jsonString);
    }

    public async Task<ComponentModel[]?> ReadAllFromJsonFile()
    {
        var jsonFileString = "[]";
        if (!fileService.FileExcist(FilePaths.ComponentPath))
        {
            fileService.WriteToJson(FilePaths.ComponentPath,jsonFileString);
            return dataConverter.DeserializeJsonStringArray(jsonFileString);
        }
        
        jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.ComponentPath);
        return dataConverter.DeserializeJsonStringArray(jsonFileString);
    }
    
    public async Task<ComponentModel?> ReadByIdFromJsonFile(int Id)
    {
        string jsonString = await fileService.ReadAllFromFileAsync(FilePaths.ComponentPath);
        return dataConverter.DeserializeJsonStringArray(jsonString)?.ToList().First(x => x.Id == Id);
    }

    public bool WriteIconToFolder(IconModel iconData)
    {
        string filePath = FilePaths.IconPath + $"{iconData.name}.{iconData.fileType}";
        return fileService.WriteAllBites(filePath, iconData.base64Data);
    }
    
    public bool DeleteIconFromFolder(IconModel iconData)
    {
        string filePath = FilePaths.IconPath + $"{iconData.name}.{iconData.fileType}";
        return fileService.FileExcist(filePath) && fileService.DeletedFile(filePath);
    }

    public bool IconExcistOnOtherComponent(List<ComponentModel>componentModels, IconModel iconData) =>
        !componentModels.Where(x => x.IconData != null &&
                                    x.IconData.name == iconData.name &&
                                    x.IconData.fileType == iconData.fileType).Any();
    
}
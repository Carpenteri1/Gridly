using Gridly.Models;

namespace Gridly.Services;

public class ComponentRepository(IDataConverter<ComponentModel> dataConverter, IFileService fileService) : IComponentRepository
{
    private const string JsonComponentFileName = "componentData.json";
    private readonly string JsonPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/ComponentData", JsonComponentFileName);
    private readonly string IconPath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Icons/");
    
    public bool ReadToJsonFile(List<ComponentModel> newComponent)
    {
        string jsonString = dataConverter.SerializerToJsonString(newComponent);
        return fileService.WriteToJson(JsonPath, jsonString);
    }

    public async Task<ComponentModel[]?> ReadAllFromJsonFile()
    {
        string jsonString = await fileService.ReadAllFromFileAsync(JsonPath);
        return dataConverter.DeserializeJsonStringArray(jsonString);
    }
    
    public async Task<ComponentModel?> ReadByIdFromJsonFile(int Id)
    {
        string jsonString = await fileService.ReadAllFromFileAsync(JsonPath);
        return dataConverter.DeserializeJsonStringArray(jsonString)?.ToList().First(x => x.Id == Id);
    }

    public bool WriteIconToFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.name}.{iconData.fileType}";
        return fileService.WriteAllBites(filePath, iconData.base64Data);
    }
    
    public bool DeleteIconFromFolder(IconModel iconData)
    {
        string filePath = IconPath + $"{iconData.name}.{iconData.fileType}";
        return fileService.FileExcist(filePath) && fileService.DeletedFile(filePath);
    }

    public bool IconExcistOnOtherComponent(List<ComponentModel>componentModels, IconModel iconData) =>
        !componentModels.Where(x => x.IconData != null &&
                                    x.IconData.name == iconData.name &&
                                    x.IconData.fileType == iconData.fileType).Any();
    
}
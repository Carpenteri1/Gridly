using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    public static IResult Save(ComponentModel newComponent)
    {
        if(newComponent.IconData != null && 
           !DataStorage.WriteIconToFolder(newComponent.IconData))
            Results.StatusCode(500);
        
        var componentModels = DataStorage.ReadFromJsonFile().Result?.ToList();
        
        if(!componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(newComponent);
        
      return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    public static IResult Edit(ComponentModel component)
    {
        if (component.IconData != null &&
            !DataStorage.WriteIconToFolder(component.IconData))
            Results.StatusCode(500);

        var componentModels = 
            DataStorage.ReadFromJsonFile().Result?.ToList();

        if (!componentModels.Any())
            componentModels = new List<ComponentModel>();

        componentModels.Select(x => x.Id == component.Id
            ? new ComponentModel
            {
                Id = component.Id,
                Name = component.Name,
                Url = component.Url,
                IconData = component.IconData,
                ImageUrl = component.ImageUrl
            }
            : component).ToList();
        
        return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    public static async Task<ComponentModel[]> Get() => 
        await DataStorage.ReadFromJsonFile();

    public static IResult Delete(int componentId)
    {
        var componentModels = DataStorage.ReadFromJsonFile()
            .Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            return Results.NotFound();
        
        var component = componentModels.FirstOrDefault(x => x.Id == componentId);
        componentModels = componentModels.Where(x => x.Id != componentId).ToList();

        if(component is null) 
            return Results.NotFound();
        
        if (component.IconData != null && 
            !componentModels.Where(x => x.IconData != null && 
                                        x.IconData.name == component.IconData.name && 
                                       x.IconData.fileType == component.IconData.fileType).Any())
        {
            if(!DataStorage.DeleteIconFromFolder(component.IconData))
                return Results.NotFound();
        }
        
        return DataStorage.ReadToJsonFile(componentModels) ? 
           Results.Ok() : Results.StatusCode(500);
    }
}
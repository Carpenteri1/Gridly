using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    public static IResult Save(ComponentModel newComponent)
    {
        if(!DataStorage.WriteIconToFolder(newComponent.IconData))
            Results.StatusCode(500);
        
        var componentModels = DataStorage.ReadFromJsonFile().Result?.ToList();
        
        if(!componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(newComponent);
        
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
        if(component is null) 
            return Results.NotFound();
        
        if (!componentModels.Any(x =>
                x.IconData.Name == component.IconData.Name &&
                x.IconData.FileType == component.IconData.FileType))
        {
            if(!DataStorage.DeleteIconFromFolder(component.IconData))
                return Results.NotFound();
        }
        
        componentModels = componentModels.Where(x => x.Id != componentId).ToList();
       
       return DataStorage.ReadToJsonFile(componentModels) ? 
           Results.StatusCode(500) : Results.Ok();
    }
}
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class SaveComponentHandler
{
    public static IResult Save(ComponentModel newComponent)
    {
        if(newComponent.IconData != null && 
           !DataStorage.WriteIconToFolder(newComponent.IconData))
            Results.StatusCode(500);
        
        var componentModels = 
            DataStorage.ReadAllFromJsonFile().Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(newComponent);
        
        return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
}
using Gridly.Services;

namespace Gridly.Handler;

public class DeleteComponentHandler
{
    public static IResult Delete(int componentId)
    {
        var componentModels = DataStorage.ReadAllFromJsonFile()
            .Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            return Results.NotFound();
        
        var component = componentModels.FirstOrDefault(x => x.Id == componentId);
        componentModels = componentModels.Where(x => x.Id != componentId).ToList();

        if(component is null) 
            return Results.NotFound();
        
        if (component.IconData != null && 
            DataStorage.IconExcistOnOtherComponent(componentModels,component.IconData))
        {
            if(!DataStorage.DeleteIconFromFolder(component.IconData))
                return Results.NotFound();
        }
        
        return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
}
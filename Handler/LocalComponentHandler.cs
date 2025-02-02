using Gridly.DTOs;
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
        
        var componentModels = 
            DataStorage.ReadAllFromJsonFile().Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(newComponent);
        
      return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    public static IResult Edit(EditComponentDto editedComponentData)
    {
        var componentModels = 
            DataStorage.ReadAllFromJsonFile().Result?.ToList();

        if (componentModels is null || !componentModels.Any())
            return Results.NotFound();
        
        if (componentModels.Count == 1 ||
            DataStorage.IconExcistOnOtherComponent(componentModels, editedComponentData.EditedIconData))
            DataStorage.DeleteIconFromFolder(editedComponentData.EditedComponent.IconData);
            
        editedComponentData.EditedComponent.IconData = editedComponentData.EditedIconData;
        for(int i = 0;i<componentModels.Count;i++)
            if (componentModels[i].Id == editedComponentData.EditedComponent.Id) 
                componentModels[i] = editedComponentData.EditedComponent;
        
        DataStorage.WriteIconToFolder(editedComponentData.EditedIconData);
        
        return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    public static async Task<ComponentModel[]?> Get() => 
        await DataStorage.ReadAllFromJsonFile();
    
    public static async Task<ComponentModel?> GetById(int Id) => 
        await DataStorage.ReadByIdFromJsonFile(Id);

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
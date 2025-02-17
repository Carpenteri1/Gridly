using Gridly.DTOs;
using Gridly.Services;

namespace Gridly.Handler;

public class EditComponentHandler
{
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
}
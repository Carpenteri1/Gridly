using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handlers;

public class ComponentHandler(IComponentRepository componentRepository) : IComponentHandler
{
    public IResult Save(ComponentModel newComponent)
    {
        if(newComponent.IconData != null && 
           !componentRepository.WriteIconToFolder(newComponent.IconData))
            Results.StatusCode(500);
        
        var componentModels = 
            componentRepository.ReadAllFromJsonFile().Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(newComponent);
        
        return componentRepository.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public IResult Delete(int componentId)
    {
        var componentModels = componentRepository.ReadAllFromJsonFile()
            .Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            return Results.NotFound();
        
        var component = componentModels.FirstOrDefault(x => x.Id == componentId);
        componentModels = componentModels.Where(x => x.Id != componentId).ToList();

        if(component is null) 
            return Results.NotFound();
        
        if (component.IconData != null && 
            componentRepository.IconExcistOnOtherComponent(componentModels,component.IconData))
        {
            if(!componentRepository.DeleteIconFromFolder(component.IconData))
                return Results.NotFound();
        }
        
        return componentRepository.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<ComponentModel[]?> GetAsync() => 
        await componentRepository.ReadAllFromJsonFile();
    
    public async Task<ComponentModel?> GetByIdAsync(int Id) => 
        await componentRepository.ReadByIdFromJsonFile(Id);
    
    public IResult Edit(EditComponentForm editedComponentForm)
    {
        var componentModels = 
            componentRepository.ReadAllFromJsonFile().Result?.ToList();

        if (componentModels is null || !componentModels.Any())
            return Results.NotFound();
        
        if (componentModels.Count == 1 ||
            componentRepository.IconExcistOnOtherComponent(componentModels, editedComponentForm.EditedIconData))
            componentRepository.DeleteIconFromFolder(editedComponentForm.EditedComponent.IconData);
            
        editedComponentForm.EditedComponent.IconData = editedComponentForm.EditedIconData;
        for(int i = 0;i<componentModels.Count;i++)
            if (componentModels[i].Id == editedComponentForm.EditedComponent.Id) 
                componentModels[i] = editedComponentForm.EditedComponent;
        
        componentRepository.WriteIconToFolder(editedComponentForm.EditedIconData);
        
        return componentRepository.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
}
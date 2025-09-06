using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class ComponentHandlerHelper(IComponentRepository componentRepository)
{
    private IEnumerable<ComponentModel> Components { get; set; }

    private bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.name) &&
        !string.IsNullOrEmpty(iconModel.type) &&
        !string.IsNullOrEmpty(iconModel.base64Data);

    private async Task<bool> IconOnOtherComponent(ComponentModel component)
    {
        var components = await GetComponents();
        return components.Any(x => x.IconData != null && 
                                   component.IconData != null && 
                                   x.Id != component.Id &&
                                   x.IconData.name == component.IconData.name && 
                                   x.IconData.type == component.IconData.type);
    }
    
    private (string, string) Split(string icon)
    {
        var split = icon.Split('.');
        return (split[0], split[1]);
    }
    
    private List<string> GetUnusedIconNames(IEnumerable<ComponentModel> componentModels) => 
        componentRepository.FindUnusedIcons(componentModels);
    private bool DeleteIcon(string name, string type) => componentRepository.DeleteIcon(name, type);

    public async Task<bool> UploadIcon(ComponentModel component)
    {
        if (!IconDataHasValue(component.IconData))
            return true;

        if (await IconOnOtherComponent(component) is not true)
            return componentRepository.UploadIcon(component.IconData);

        return true;
    }
    public async Task<bool> DeleteIcon(ComponentModel component)
    {
        if (IconDataHasValue(component.IconData))
        {
            if (await IconOnOtherComponent(component) is not true)
                return componentRepository.DeleteIcon(component.IconData.name, component.IconData.type);   
        }
        return true;
    }
    
    public async Task<bool> DeleteUnUsedIcon()
    {
        var componenets = await GetComponents();
        foreach (var icon in GetUnusedIconNames(componenets))                      
        {                                                                                                          
            var (name,type) = Split(icon);                                                           
            if (!DeleteIcon(name,type))                                                              
                return false;                                                        
        }

        return true;
    }

    public async Task<bool> UpdateComponent(ComponentModel editComponent)
    {
        var components = await GetComponents();
        var componentsList = components.ToList();
        var index = componentsList.FindIndex(0, x => x.Id == editComponent.Id);

        if (index != -1)
        {
            componentsList[index] = editComponent;
        }
        
        Components = componentsList.AsEnumerable();
        return Components.Contains(Components.ToList()[index]); 
    }
    
    public async Task<bool> BatchUpdateComponent(IEnumerable<ComponentModel> editedComponents)
    {
        var listOfComponents = await GetComponents();
        var listOfEditedComponents = editedComponents.ToList();
        var exceptions = !listOfComponents.Except(listOfEditedComponents).Any();
        
        if(exceptions)
            return exceptions;
        
        Components = listOfEditedComponents.AsEnumerable();
        return exceptions;
    }
    
    private async Task<IEnumerable<ComponentModel>> GetComponents()
    {
        if (Components == null || !Components.Any())
        {
            Components = await componentRepository.Get();            
        }
        return Components;
    }
    
    public async Task<ComponentModel> GetComponentById(int Id) => await componentRepository.GetById(Id);
}
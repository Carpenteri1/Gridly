using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class ComponentHandlerHelper(IComponentRepository componentRepository, IIconRepository iconRepository, IFileService fileService)
{
    private IEnumerable<ComponentModel> Components { get; set; }

    public bool IconDataHasValue(IconModel iconModel) =>
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
    
    private bool DeleteIcon(string name, string type) => fileService.DeleteIcon(name, type);

    public async Task<bool> UploadIcon(ComponentModel component) 
        =>  fileService.UploadIcon(component.IconData);
            //await IconOnOtherComponent(component) is false &&  
            
    public async Task<bool> DeleteIcon(ComponentModel component)
    {
        if (IconDataHasValue(component.IconData))
        {
            if (await IconOnOtherComponent(component) is false)
                return fileService.DeleteIcon(component.IconData.name, component.IconData.type);   
        }
        return true;
    }
    
    public async Task<bool> DeleteUnUsedIcon()
    {
        var componenets = await GetComponents();
        foreach (var icon in iconRepository.FindUnusedIcons(componenets))                      
        {                                                                                                          
            var (name,type) = Split(icon);                                                           
            if (!DeleteIcon(name,type))                                                              
                return false;                                                        
        }

        return true;
    }
    
    private async Task<IEnumerable<ComponentModel>> GetComponents()
    {
        if (Components == null || !Components.Any())
        {
            Components = await componentRepository.Get();            
        }
        return Components;
    }
}
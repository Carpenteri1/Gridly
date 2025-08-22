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

    private bool IconOnOtherComponent(IconModel iconData)
        => GetComponents().Any(x => x.IconData != null && 
                                      x.IconData.name == iconData.name &&
                                      x.IconData.type == iconData.type);
    
    private (string, string) Split(string icon)
    {
        var split = icon.Split('.');
        return (split[0], split[1]);
    }
    
    private List<string> GetUnusedIconNames(IEnumerable<ComponentModel> componentModels) => 
        componentRepository.FindUnusedIcons(componentModels);
    private bool DeleteIcon(string name, string type) => componentRepository.DeleteIcon(name, type);

    public bool UploadIcon(ComponentModel component)
    {
        if (!IconDataHasValue(component.IconData))
            return true;

        return !IconOnOtherComponent(component.IconData) &&
               componentRepository.UploadIcon(component.IconData);
    }
    public bool DeleteIcon(ComponentModel component)
    {
        if (IconDataHasValue(component.IconData))
        {
            if (!IconOnOtherComponent(component.IconData))
                return componentRepository.DeleteIcon(component.IconData.name, component.IconData.type);   
        }
        return true;
    }
    
    public bool DeleteUnUsedIcon()
    {
        foreach (var icon in GetUnusedIconNames(GetComponents()))                      
        {                                                                                                          
            var (name,type) = Split(icon);                                                           
            if (!DeleteIcon(name,type))                                                              
                return false;                                                        
        }

        return true;
    }
    
    public bool DeleteComponent(ComponentModel component)
    {
        Components = GetComponents().ToList().Where(x => x.Id != component.Id).AsEnumerable();
        return !Components.ToList().Contains(component);
    }

    public bool UpdateComponent(ComponentModel editComponent)
    {
        var componentsList = GetComponents().ToList();
        var index = componentsList.FindIndex(0, x => x.Id == editComponent.Id);

        if (index != -1)
        {
            componentsList[index] = editComponent;
        }
        Components = componentsList.AsEnumerable();
        return Components.Contains(Components.ToList()[index]); 
    }
    
    public bool BatchUpdateComponent(IEnumerable<ComponentModel> editedComponents)
    {
        Components = GetComponents();
        Components = editedComponents;
        
        return Components.Count() == editedComponents.Count() &&
            !Components.Except(editedComponents).Any() &&
            !editedComponents.Except(Components).Any() && 
            !Components.SequenceEqual(editedComponents);
    }
    
    public IEnumerable<ComponentModel> GetComponents()
    {
        if (Components == null || !Components.Any())
        {
            Components = componentRepository.Get().Result;            
        }
        return Components;
    }
    
    public ComponentModel GetComponentById(int Id) => 
        GetComponents().First(x => x.Id == Id);
    
    public bool AddComponent(ComponentModel component)
    {
        int amountOfComponents = GetComponents().Count();
        var components = GetComponents().ToList();
        components.Add(component);
        Components = components;
        return Components.Count() == amountOfComponents + 1;
    }
}
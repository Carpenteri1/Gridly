using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class ComponentHandlerHelper(IComponentRepository componentRepository, IIconRepository iconRepository, IFileService fileService)
{
    private IEnumerable<ComponentModel> Components { get; set; }

    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.Name) &&
        !string.IsNullOrEmpty(iconModel.Type) &&
        !string.IsNullOrEmpty(iconModel.Base64Data);
    
    public bool UploadIcon(ComponentModel component) 
        =>  fileService.UploadIcon(component.IconData);
            
    public bool DeleteIcon(ComponentModel component) =>
        fileService.DeleteIcon(component.IconData.Name, component.IconData.Type);   
    
    private async Task<IEnumerable<ComponentModel>> GetComponents()
    {
        if (Components == null || !Components.Any())
        {
            Components = await componentRepository.Get();            
        }
        return Components;
    }
}
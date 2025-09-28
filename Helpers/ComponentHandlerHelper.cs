using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class ComponentHandlerHelper(IFileService fileService)
{
    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.Name) &&
        !string.IsNullOrEmpty(iconModel.Type) &&
        !string.IsNullOrEmpty(iconModel.Base64Data);
    
    public bool UploadIcon(ComponentModel component) 
        =>  fileService.UploadIcon(component.IconData);
            
    public bool DeleteIcon(ComponentModel component) =>
        fileService.DeleteIcon(component.IconData.Name, component.IconData.Type);
}
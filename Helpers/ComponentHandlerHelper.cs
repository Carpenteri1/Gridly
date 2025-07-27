using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class ComponentHandlerHelper(IComponentRepository componentRepository)
{
    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.name) &&
        !string.IsNullOrEmpty(iconModel.type) &&
        !string.IsNullOrEmpty(iconModel.base64Data);

    public bool UploadIcon(ComponentModel component, IEnumerable<ComponentModel> componentModels)
    {
        if (!componentRepository.IconDuplicate(componentModels, component.IconData))
            return componentRepository.UploadIcon(component.IconData);
        
        return true;
    }

    public bool DeleteIcon(ComponentModel component, IEnumerable<ComponentModel> componentModels)
    {
        if (!componentRepository.IconDuplicate(componentModels, component.IconData) || componentModels.Count() == 1)
            return componentRepository.DeleteIcon(component.IconData);
        
        return true;
    }
}
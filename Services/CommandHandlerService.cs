using Gridly.Models;

namespace Gridly.Services;

public class CommandHandlerService(IComponentRepository componentRepository) : ICommandHandlerService
{
    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        iconModel.name != string.Empty && iconModel.name != null &&
        iconModel.type != string.Empty && iconModel.type != null &&
        iconModel.base64Data != string.Empty && iconModel.base64Data != null;

    public bool UploadIcon(ComponentModel component, IEnumerable<ComponentModel> componentModels)
    {
        if (componentRepository.IconDuplicate(componentModels, component.IconData) && 
            !componentRepository.DeleteIcon(component.IconData))
            return false;
        
        return componentRepository.UploadIcon(component.IconData);
    }
}
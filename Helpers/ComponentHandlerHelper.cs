using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class CardHandlerHelper(IFileService fileService)
{
    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.Name) &&
        !string.IsNullOrEmpty(iconModel.Type) &&
        !string.IsNullOrEmpty(iconModel.Base64Data);
            
    public bool DeleteIcon(IconModel iconData) =>
        fileService.DeleteIcon(iconData.Name, iconData.Type);
}
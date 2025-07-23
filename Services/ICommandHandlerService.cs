using Gridly.Models;

namespace Gridly.Services;

public interface ICommandHandlerService
{
        bool IconDataHasValue(IconModel iconModel);
        bool UploadIcon(ComponentModel component, IEnumerable<ComponentModel> componentModels);
}
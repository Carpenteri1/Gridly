using Gridly.Command;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ComponentHandler(IComponentRepository componentRepository, IFileService fileService) : 
    IRequestHandler<SaveComponentCommand, IResult>,
    IRequestHandler<GetAllComponentCommand, IResult>,
    IRequestHandler<GetByIdComponentCommands, ComponentModel>,
    IRequestHandler<DeleteComponentCommand, IResult>,
    IRequestHandler<EditComponentCommand, IResult>,
    IRequestHandler<BatchEditComponentCommand, IResult>
{
    private readonly ComponentHandlerHelper handlerHelper = new(componentRepository, fileService);
    public async Task<IResult> Handle(SaveComponentCommand command, CancellationToken cancellationToken)
    {
        command.ComponentSettings = new ComponentSettingsModel(componentId: command.Id, width: 200, height: 200);
        return componentRepository.Insert(command) ?
        Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var component = await handlerHelper.GetComponentById(command.Id);
        if(await handlerHelper.DeleteIcon(component) is false || 
           !componentRepository.Delete(component))
            return Results.NotFound();
        
        componentRepository.Delete(component);
        
        return componentRepository.Insert(command) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        switch (command.SelectedDropDownIconValue)
        {
            case 0:
                if (await handlerHelper.UpdateComponent(command.EditComponent!) is false)
                    return Results.NotFound();  
                break;
            case 1:
                if(await handlerHelper.UploadIcon(command.EditComponent) is false) 
                    return Results.NotFound();
                
                command.EditComponent.IconUrl = string.Empty;                
                if (await handlerHelper.UpdateComponent(command.EditComponent!) is false)  
                    return Results.NotFound();              
                
                if (await handlerHelper.DeleteUnUsedIcon() is false || 
                    await handlerHelper.UpdateComponent(command.EditComponent!) is false) 
                    return Results.NotFound();
                break;
            case 2: 
                command.EditComponent.IconData = null;                
                if (await handlerHelper.UpdateComponent(command.EditComponent!) is false ||
                    await handlerHelper.DeleteUnUsedIcon() is false)    
                    return Results.NotFound();
                break;
        }

        return componentRepository.Insert(command.EditComponent) ? 
            Results.Ok() : Results.StatusCode(500);
    }

    public async Task<IResult> Handle(BatchEditComponentCommand commands, CancellationToken cancellationToken)
    {
        if (await handlerHelper.BatchUpdateComponent(commands) is not true)
            return Results.NotFound();
        
        return componentRepository.Insert(commands) 
            ? Results.Ok() : Results.StatusCode(500);                                         
    }

    public async Task<IResult> Handle(GetAllComponentCommand command, CancellationToken cancellationToken)
    {
        var components = await componentRepository.Get();
        return Results.Ok(components) ?? Results.Empty;
    }
    
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands command, CancellationToken cancellationToken) => 
        await componentRepository.GetById(command.Id);
}
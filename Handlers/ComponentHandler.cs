using Gridly.Command;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ComponentHandler(IComponentRepository componentRepository) : 
    IRequestHandler<SaveComponentCommand, IResult>,
    IRequestHandler<GetAllComponentCommand, ComponentModel[]>,
    IRequestHandler<GetByIdComponentCommands, ComponentModel>,
    IRequestHandler<DeleteComponentCommand, IResult>,
    IRequestHandler<EditComponentCommand, IResult>,
    IRequestHandler<BatchEditComponentCommand, IResult>
{
    private readonly ComponentHandlerHelper handlerHelper = new(componentRepository);
    public Task<IResult> Handle(SaveComponentCommand command, CancellationToken cancellationToken)
    {
        if(!handlerHelper.UploadIcon(command))
            return Task.FromResult(Results.NotFound());   
        
        command.ComponentSettings = new ComponentSettingsModel(200, 200);
        return Task.FromResult(handlerHelper.AddComponent(command) && componentRepository.Save(handlerHelper.GetComponents()) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var component = handlerHelper.GetComponentById(command.Id);
        if(!handlerHelper.DeleteIcon(component))
            return Task.FromResult(Results.NotFound());
        
        if(!handlerHelper.DeleteComponent(component))
            return Task.FromResult(Results.NotFound());
        
        return Task.FromResult(componentRepository.Save(handlerHelper.GetComponents()) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        switch (command.SelectedDropDownIconValue)
        {
            case 0:
                if (!handlerHelper.UpdateComponent(command.EditComponent!))
                    return Task.FromResult(Results.NotFound());  
                break;
            case 1:
                if(!handlerHelper.UploadIcon(command.EditComponent)) 
                    return Task.FromResult(Results.NotFound());
                
                command.EditComponent.IconUrl = string.Empty;                
                if (!handlerHelper.UpdateComponent(command.EditComponent!))  
                    return Task.FromResult(Results.NotFound());              
                
                if (!handlerHelper.DeleteUnUsedIcon()) 
                    return Task.FromResult(Results.NotFound());
                break;
            case 2: 
                command.EditComponent.IconData = null;                
                if (!handlerHelper.UpdateComponent(command.EditComponent!))    
                    return Task.FromResult(Results.NotFound());                
                
                if(!handlerHelper.DeleteUnUsedIcon())
                    return Task.FromResult(Results.NotFound()); 
                break;
        }

        return Task.FromResult(componentRepository.Save(handlerHelper.GetComponents()) ? 
            Results.Ok() : Results.StatusCode(500));
    }

    public Task<IResult> Handle(BatchEditComponentCommand commands, CancellationToken cancellationToken)
    {
        if (!handlerHelper.BatchUpdateComponent(commands))
            return Task.FromResult(Results.NotFound());
        
        return Task.FromResult(componentRepository.Save(handlerHelper.GetComponents()) 
            ? Results.Ok()                                                      
            : Results.StatusCode(500));                                         
    }
    
    public async Task<ComponentModel[]> Handle(GetAllComponentCommand command, CancellationToken cancellationToken) =>
        (await componentRepository.Get()).ToArray();
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands command, CancellationToken cancellationToken) => 
        await componentRepository.GetById(command.Id);
}
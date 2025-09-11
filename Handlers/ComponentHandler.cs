using Gridly.Command;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ComponentHandler(
    IComponentRepository componentRepository, 
    IComponentSettingsRepository settingsRepository,
    IIconRepository iconRepository,
    IIconConnectedRepository iconConnectedRepository,
    IFileService fileService) : 
    IRequestHandler<SaveComponentCommand, IResult>,
    IRequestHandler<GetAllComponentCommand, IResult>,
    IRequestHandler<GetByIdComponentCommands, ComponentModel>,
    IRequestHandler<DeleteComponentCommand, IResult>,
    IRequestHandler<EditComponentCommand, IResult>,
    IRequestHandler<BatchEditComponentCommand, IResult>
{
    private readonly ComponentHandlerHelper handlerHelper = new(componentRepository,iconRepository, fileService);
    public async Task<IResult> Handle(SaveComponentCommand command, CancellationToken cancellationToken)
    {
        command.ComponentSettings = 
            new ComponentSettingsModel(componentId: command.Id, width: 200, height: 200);
        
        if (await componentRepository.Insert(command) is false && 
            await settingsRepository.Insert(command.ComponentSettings) is false)
            return Results.StatusCode(500);

        if (handlerHelper.IconDataHasValue(command.IconData) && 
           string.IsNullOrEmpty(command.IconUrl))
        {
            var connectedModel = await iconConnectedRepository.GetById(command.Id, command.IconData.id);
                if (connectedModel == null) 
                {
                    await iconConnectedRepository.Insert(connectedModel);
                }
            
            return Results.Ok();
        }
        if (handlerHelper.IconDataHasValue(command.IconData) is false && 
           string.IsNullOrEmpty(command.IconUrl) is false)
        {
            var icon = await iconRepository.GetById(command.Id);
            if (icon != null)
            {
                var iconsConnected = await iconConnectedRepository.GetManyById(null,icon.id);
                if (!iconsConnected.Any())
                {
                    await iconConnectedRepository.Delete(command.Id);
                    await iconRepository.Delete(icon.id);
                    fileService.DeleteIcon(icon.name, icon.type);
                }   
            }
        }
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
                
        await componentRepository.Delete(command);
        await settingsRepository.Delete(command.IconData.id);

        if (command.IconData != null)
        {
            await iconConnectedRepository.Delete(command.Id);
            await iconRepository.Delete(command.IconData.id);
            
            var iconsConnected = 
                await iconConnectedRepository.GetManyById(null,command.IconData.id);
            
            if (!iconsConnected.Any())
            {
                await iconRepository.Delete(command.IconData.id);
                await iconConnectedRepository.Delete(command.Id);
                fileService.DeleteIcon(command.IconData.name, command.IconData.type);
            }   
        }
        
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        switch (command.SelectedDropDownIconValue)
        {
            case 1:
                command.EditComponent.IconUrl = string.Empty;
                if (await handlerHelper.DeleteUnUsedIcon() &&
                    await handlerHelper.UploadIcon(command.EditComponent) is false) 
                    return Results.StatusCode(500);
                break;
            case 2: 
                if (await handlerHelper.DeleteUnUsedIcon() is false)
                    return Results.NotFound();
                break;
        }
        
        return await componentRepository.Edit(command.EditComponent) ? 
            Results.Ok() : Results.StatusCode(500);
    }

    public async Task<IResult> Handle(BatchEditComponentCommand commands, CancellationToken cancellationToken) 
        => await componentRepository.BatchEdit(commands) ? Results.Ok() : Results.StatusCode(500);                                         

    public async Task<IResult> Handle(GetAllComponentCommand command, CancellationToken cancellationToken)
    {
        var components = await componentRepository.Get();
        return components != null ? Results.Ok(components) : Results.NotFound();
    }
    
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands command, CancellationToken cancellationToken) => 
        await componentRepository.GetById(command.Id);
}
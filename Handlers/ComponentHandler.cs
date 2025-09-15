using Gridly.Command;
using Gridly.Dtos;
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
        var component = await componentRepository.Insert(command);
        component.ComponentSettings = await settingsRepository.Insert(
            new ComponentSettingsModel{ComponentId = command.Id, Width = 200, Height = 200});
        
        if (component == null || component.ComponentSettings == null)
            return Results.StatusCode(500);
        
        if (handlerHelper.IconDataHasValue(command.IconData) && 
             string.IsNullOrEmpty(command.IconUrl))
        { 
            component.IconData = await iconRepository.GetByFullName(command.IconData);
            var connectionModel = new IconConnectedDtoModel { ComponentId = component.Id };
             if (component.IconData != null)
             {
                 var connectedDtoModels = await iconConnectedRepository.GetManyById(null, component.IconData.Id);
                 if (connectedDtoModels.Any(x => x.IconId == component.IconData.Id))
                 {
                     connectionModel.IconId = component.IconData.Id;
                     return await iconConnectedRepository.Insert(connectionModel) != null ? 
                         Results.Ok() : Results.StatusCode(500);
                 }
             }
             
             component.IconData = await iconRepository.Insert(command.IconData);
             if (component.IconData != null) connectionModel.IconId = component.IconData.Id;
             if(handlerHelper.UploadIcon(component) && 
                await iconConnectedRepository.Insert(connectionModel) == null)
                 return Results.StatusCode(500);
        }
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var component = await componentRepository.GetById(command.Id);
        if(await settingsRepository.Delete(component.ComponentSettings.Id.Value) is false &&
            await componentRepository.Delete(component) is false)
            return Results.StatusCode(500);

        if (component.IconData != null)
        {
            var iconsConnected = 
                await iconConnectedRepository.GetManyById(null,component.IconData.Id);
            
            if (!iconsConnected.Any())
            {
                await iconRepository.Delete(component.IconData.Id);
                await iconConnectedRepository.Delete(command.Id);
                fileService.DeleteIcon(component.IconData.Name, component.IconData.Type);
            }   
        }
        
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        var component = await componentRepository.GetById(command.EditComponent.Id);
        if(component == null) return Results.NotFound();
        
        var connectionModel = new IconConnectedDtoModel { ComponentId = component.Id };
        var editHasIcon = handlerHelper.IconDataHasValue(command.EditComponent.IconData);
        var currentHasIcon = handlerHelper.IconDataHasValue(component.IconData);


        switch (command.SelectedDropDownIconValue)
        {
            case 1:
                component.IconUrl = string.Empty;
                if (editHasIcon)
                {
                    if (currentHasIcon)
                    {
                        var icon = await iconRepository.GetByFullName(component.IconData);
                        if (icon != null)
                        {
                            var connections = await iconConnectedRepository.GetManyById(null, icon.Id);
                            if (connections.Count(x => x.IconId == icon.Id) <= 1)
                            {
                                await iconConnectedRepository.Delete(component.Id);
                                await iconRepository.Delete(icon.Id);
                                handlerHelper.DeleteIcon(component);
                            }
                        }
                    }
                    var editIcon = await iconRepository.GetByFullName(command.EditComponent.IconData);
                    if (editIcon == null)
                    {
                        editIcon = await iconRepository.Insert(command.EditComponent.IconData);
                        component.IconData = editIcon;
                        connectionModel.IconId = editIcon.Id;
                        await iconConnectedRepository.Insert(connectionModel);
                        handlerHelper.UploadIcon(component);
                    }
                    else
                    {
                        component.IconData = editIcon;
                        var connections = await iconConnectedRepository.GetManyById(null, editIcon.Id);
                        if (connections.Count(x => x.IconId == editIcon.Id) == 0)
                        {
                            connectionModel.IconId = editIcon.Id;
                            if (await iconConnectedRepository.Insert(connectionModel) == null) return Results.StatusCode(500);   
                        }
                    }
                }
                else
                {
                    connectionModel.ComponentId = component.Id;
                    component.IconData = await iconRepository.Insert(command.EditComponent.IconData);
                    if (component.IconData != null) connectionModel.IconId = component.IconData.Id;
                    if(handlerHelper.UploadIcon(component) && 
                       await iconConnectedRepository.Insert(connectionModel) == null)
                        return Results.StatusCode(500);   
                }
                break;
            case 2:
                if (currentHasIcon)
                {
                    var connections = await iconConnectedRepository.GetManyById(null, component.IconData.Id);
                    var connectionsToIcon = connections.Count(x => x.IconId == component.IconData.Id);
                    if (connectionsToIcon == 1)
                    {
                        if(handlerHelper.DeleteIcon(component) is false &&
                           await iconRepository.Delete(component.IconData.Id) is false)
                            return Results.StatusCode(500);  
                    }
                    if (await iconConnectedRepository.Delete(component.Id) is false) return Results.StatusCode(500);  
                    component.IconUrl = command.EditComponent.IconUrl;
                } 
                break;
        }
        
        return await componentRepository.Edit(component) ? 
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
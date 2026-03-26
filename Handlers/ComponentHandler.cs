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
    private readonly ComponentHandlerHelper handlerHelper = new(fileService);
    public async Task<IResult> Handle(SaveComponentCommand command, CancellationToken cancellationToken)
    {
        var storedComponents = await componentRepository.Get();
        
        if (storedComponents == null || storedComponents.Count() == 0 ) command.IndexPosition = 1;
        else command.IndexPosition = storedComponents.Count() + 1;
        
        var component = await componentRepository.Insert(command);
        command.ComponentSettings.ComponentId = component.Id;
        component.ComponentSettings = await settingsRepository.Insert(command.ComponentSettings);
        
        if (component == null || component.ComponentSettings == null)
            return Results.StatusCode(500);
        
        return Results.Ok();
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var components = await componentRepository.Get();
   
        
        if (await settingsRepository.Delete(command.Id) is false ||
            await componentRepository.Delete(command.Id) is false)
            return Results.StatusCode(500);
        
         var component = components.FirstOrDefault(x => x.Id == command.Id);
        if (component != null &&
         component.IconData != null)
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
        
        components = await componentRepository.Get();
        var indexedComponents = handlerHelper.SetIndexValues(components.ToList());
        return await componentRepository.BatchEdit(indexedComponents) == false ? Results.StatusCode(500) : Results.Ok();
    }
    
    public async Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        var components = await componentRepository.Get();
        if (components == null) return Results.StatusCode(500);
        
        var component = components.FirstOrDefault(x => x.Id == command.EditComponent.Id);
        
        var connectionModel = new IconConnectedDtoModel { ComponentId = component.Id };
        var editHasIcon = handlerHelper.IconDataHasValue(command.EditComponent.IconData);
        var currentHasIcon = handlerHelper.IconDataHasValue(component.IconData);


         //switch (command.SelectedDropDownIconValue)
        // {
           //  case 1:
                 component.IconUrl = string.Empty;
                 /*if (editHasIcon)
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
                 }*/

             //    break;
           //  case 2:
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
                        //component.IconUrl = command.EditComponent.IconUrl;
                        component.IconData.MaterialIcon = command.EditComponent.IconData.MaterialIcon;
        } 
               // break;
        // }

        if(component.IconData is not null)
        {
            component.IconData.MaterialIcon = command.EditComponent.IconData.MaterialIcon;
        }
        else
        {
            component.IconData = new IconModel
            {
                Id = command.EditComponent.IconData.Id,
                Name = command.EditComponent.IconData.Name,
                Type = command.EditComponent.IconData.Type,
                Base64Data = command.EditComponent.IconData.Base64Data,
                MaterialIcon = command.EditComponent.IconData.MaterialIcon
            };
        }

        if (component != command.EditComponent)
        {
            component.Name = command.EditComponent.Name;
            component.Url = command.EditComponent.Url;
        }

        if (component.ComponentSettings != command.EditComponent.ComponentSettings)
        {
            component.ComponentSettings.ImageHidden = command.EditComponent.ComponentSettings.ImageHidden;
            component.ComponentSettings.TitleHidden = command.EditComponent.ComponentSettings.TitleHidden;
            component.ComponentSettings.Height = command.EditComponent.ComponentSettings.Height;
            component.ComponentSettings.Width = command.EditComponent.ComponentSettings.Width;
        }

        var result = await settingsRepository.Edit(component.ComponentSettings) != null;
        result = await componentRepository.Edit(component);
        component.IconData = await iconRepository.Insert(component.IconData);
        if (component.IconData.Id > 0)
        {
            result = await iconConnectedRepository.Insert(new IconConnectedDtoModel
            {
                ComponentId = component.Id,
                IconId = component.IconData.Id
            }) != null;
        }
        return result ? Results.Ok(): Results.StatusCode(500);
    }

    public async Task<IResult> Handle(BatchEditComponentCommand commands, CancellationToken cancellationToken)
    {
        var sortedComponents = handlerHelper.SetIndexValues(commands);
        if(sortedComponents == null) return Results.StatusCode(500);
        
        return await componentRepository.BatchEdit(sortedComponents) ? 
            Results.Ok() : Results.StatusCode(500);                                         
    }

    public async Task<IResult> Handle(GetAllComponentCommand command, CancellationToken cancellationToken)
    {
        var components = await componentRepository.Get();
        return Results.Ok(components);
    }
    
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands command, CancellationToken cancellationToken) => 
        await componentRepository.GetById(command.Id);
}

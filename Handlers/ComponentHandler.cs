using Gridly.Command;
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
    public async Task<IResult> Handle(SaveComponentCommand commands, CancellationToken cancellationToken)
    {
        var componentModels = 
            componentRepository.Get().Result?.ToList();

        if (commands.ImageUrl != null && 
            commands.ImageUrl != string.Empty)
        {
            if (commands.IconData != null && 
                componentRepository.IconDuplicate(componentModels,commands.IconData))
            {
                if(!componentRepository.DeleteIcon(commands.IconData))
                    return Results.NotFound();
                
                commands.IconData = null;
            }
        }
        else if(commands.IconData != null && 
                commands.IconData.name != string.Empty && commands.IconData.name != null &&
                commands.IconData.type != string.Empty && commands.IconData.type != null &&
                commands.IconData.base64Data != string.Empty && commands.IconData.base64Data != null)
        {
            if (!componentRepository.IconDuplicate(componentModels, commands.IconData))
            {
                componentRepository.UploadIcon(commands.IconData);
                commands.ImageUrl = string.Empty;
            }   
        }
        
        if (commands.IconData != null &&
            !componentRepository.UploadIcon(commands.IconData))
            return Results.NotFound();
        
        if (commands.ComponentSettings == null)
            commands.ComponentSettings = new ComponentSettingsModel(200, 200);
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(commands);
        
        return componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var componentModels = await
            componentRepository
                .Get();
        
        if(componentModels is null || !componentModels.Any()) 
            return Results.NotFound();
        
        var component = componentModels.FirstOrDefault(x => x.Id == command.Id);
        componentModels = componentModels.Where(x => x.Id != command.Id).ToList();

        if (component is null) 
            return Results.NotFound();
        
        if (component.IconData != null && 
            !componentRepository.IconDuplicate(componentModels,component.IconData))
        {
            if(!componentRepository.DeleteIcon(component.IconData))
                return Results.NotFound();
        }
        
        return componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        var componentModels = (await componentRepository.Get()).ToArray();
        
        if (componentModels is null || !componentModels.Any())
            return Results.NotFound();

        if (command.ImageUrl != null && 
            command.ImageUrl != string.Empty)
        {
            if (command.IconData != null && 
                componentRepository.IconDuplicate(componentModels,command.IconData))
            {
                if(!componentRepository.DeleteIcon(command.IconData))
                    return Results.NotFound();
                
                command.IconData = null;
            }
        }
        else if(command.IconData != null && 
                command.IconData.name != string.Empty && command.IconData.name != null &&
                command.IconData.type != string.Empty && command.IconData.type != null &&
                command.IconData.base64Data != string.Empty && command.IconData.base64Data != null)
        {
            if (!componentRepository.IconDuplicate(componentModels, command.IconData))
            {
                componentRepository.UploadIcon(command.IconData);
                command.ImageUrl = string.Empty;
            }   
        }
        
        for(int i = 0;i<componentModels.Length;i++)
            if (componentModels[i].Id == command.Id) 
                componentModels[i] =  command;
        
        return componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<IResult> Handle(BatchEditComponentCommand command, CancellationToken cancellationToken)
    {
        var componentModels = (await componentRepository.Get()).ToArray();
        
        if (componentModels is null || !componentModels.Any())
            return Results.NotFound();

        foreach (var c in command)
        {
            if (c.ImageUrl != null || 
                c.ImageUrl != string.Empty)
            {
                if (c.IconData != null && !componentRepository.IconDuplicate(componentModels, c.IconData))
                {
                    if(!componentRepository.DeleteIcon(c.IconData))
                        return Results.NotFound();
                    if (c.IconData != null && !componentRepository.UploadIcon(c.IconData))
                        return Results.StatusCode(500);
                }
            }
        }

        for (int i = 0; i < componentModels.Length; i++)
        {
            var index = command.IndexOf(componentModels[i]);
            if (index != i)
            {
                componentModels = command.ToArray();
            }
            
            var c = command.Find(x => x.Id == componentModels[i].Id);
            if (c != null && componentModels[i].Id == c.Id)
                componentModels[i] = c;
        }
        
        return componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<ComponentModel[]> Handle(GetAllComponentCommand command, CancellationToken cancellationToken) =>
        (await componentRepository.Get()).ToArray();
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands request, CancellationToken cancellationToken) => 
        await componentRepository.GetById(request.Id);
}
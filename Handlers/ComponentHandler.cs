using Gridly.Command;
using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ComponentHandler(IComponentRepository componentRepository) : 
    IRequestHandler<SaveComponentCommand, ComponentModel[]>,
    IRequestHandler<GetAllComponentCommand, ComponentModel[]>,
    IRequestHandler<GetByIdComponentCommands, ComponentModel>,
    IRequestHandler<DeleteComponentCommand, IResult>,
    IRequestHandler<EditComponentCommand, IResult>,
    IRequestHandler<BatchEditComponentCommand, IResult>
{
    public Task<ComponentModel[]> Handle(SaveComponentCommand commands, CancellationToken cancellationToken)
    {
        if (commands.IconData != null &&
            !componentRepository.UploadIcon(commands.IconData))
            return Task.FromResult<ComponentModel[]>(null);

        if (commands.ComponentSettings == null)
            commands.ComponentSettings = new ComponentSettingsModel(200, 200);

        var componentModels = 
            componentRepository.Get().Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(commands);
        componentRepository.Save(componentModels);
        return Task.FromResult(componentModels.ToArray());
    }
    
    public async Task<IResult> Handle(DeleteComponentCommand commands, CancellationToken cancellationToken)
    {
        var componentModels = await
            componentRepository
                .Get();
        
        if(componentModels is null || !componentModels.Any()) 
            return Results.NotFound();
        
        var component = componentModels.FirstOrDefault(x => x.Id == commands.Id);
        componentModels = componentModels.Where(x => x.Id != commands.Id).ToList();

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

        if (command.EditedComponent.ImageUrl != null || 
            command.EditedComponent.ImageUrl != string.Empty)
        {
            if (command.EditedComponent.IconData != null && 
                !componentRepository.IconDuplicate(componentModels,command.EditedComponent.IconData))
            {
                if(!componentRepository.DeleteIcon(command.EditedComponent.IconData))
                    return Results.NotFound();
            }
        }
        else
        {
            if (!componentRepository.IconDuplicate(componentModels, command.EditedComponent.IconData))
            {
                componentRepository.UploadIcon(command.EditedComponent.IconData);
                command.EditedComponent.ImageUrl = string.Empty;
            }   
        }
        
        for(int i = 0;i<componentModels.Length;i++)
            if (componentModels[i].Id == command.EditedComponent.Id) 
                componentModels[i] =  command.EditedComponent;
        
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
                if (c.IconData != null && 
                    !componentRepository.IconDuplicate(componentModels, c.IconData))
                {
                    if(!componentRepository.DeleteIcon(c.IconData))
                        return Results.NotFound();
                }
            }
            else
            {
                if (!componentRepository.IconDuplicate(componentModels, c.IconData))
                {
                    componentRepository.UploadIcon(c.IconData);
                    c.ImageUrl = string.Empty;
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
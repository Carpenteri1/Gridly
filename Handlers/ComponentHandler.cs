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
    IRequestHandler<EditComponentCommand, IResult>
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

        if (!componentRepository.IconDuplicate(componentModels,command.EditedComponent.IconData))
            componentRepository.UploadIcon(command.EditedComponent.IconData);
        
        for(int i = 0;i<componentModels.Length;i++)
            if (componentModels[i].Id == command.EditedComponent.Id) 
                componentModels[i] =  command.EditedComponent;
        
        return componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);
    }
    
    public async Task<ComponentModel[]> Handle(GetAllComponentCommand command, CancellationToken cancellationToken) =>
        (await componentRepository.Get()).ToArray();
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands request, CancellationToken cancellationToken) => 
        await componentRepository.GetById(request.Id);
}
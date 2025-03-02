using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ComponentHandler(IComponentRepository componentRepository) : 
    IRequestHandler<SaveCommand, ComponentModel[]>,
    IRequestHandler<GetAllCommand, ComponentModel[]>,
    IRequestHandler<GetByIdCommand, ComponentModel>,
    IRequestHandler<DeleteCommand, IResult>,
    IRequestHandler<EditCommand, IResult>
{
    public Task<ComponentModel[]> Handle(SaveCommand command, CancellationToken cancellationToken)
    {
        if (command.IconData != null &&
            !componentRepository.WriteIconToFolder(command.IconData))
            return Task.FromResult<ComponentModel[]>(null);

        var componentModels = 
            componentRepository.ReadAllFromJsonFile().Result?.ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            componentModels = new List<ComponentModel>();
        
        componentModels.Add(command);
        componentRepository.ReadToJsonFile(componentModels);
        return Task.FromResult(componentModels.ToArray());
    }
    
    public Task<IResult> Handle(DeleteCommand command, CancellationToken cancellationToken)
    {
        var componentModels = 
            componentRepository
                .ReadAllFromJsonFile()
                .Result?
                .ToList();
        
        if(componentModels is null || !componentModels.Any()) 
            return Task.FromResult(Results.NotFound());
        
        var component = componentModels.FirstOrDefault(x => x.Id == command.Id);
        componentModels = componentModels.Where(x => x.Id != command.Id).ToList();

        if (component is null) 
            return Task.FromResult(Results.NotFound());
        
        if (component.IconData != null && 
            componentRepository.IconExcistOnOtherComponent(componentModels,component.IconData))
        {
            if(!componentRepository.DeleteIconFromFolder(component.IconData))
                return Task.FromResult(Results.NotFound());
        }
        
        return Task.FromResult(componentRepository.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(EditCommand command, CancellationToken cancellationToken)
    {
        var componentModels = 
            componentRepository.ReadAllFromJsonFile().Result?.ToList();

        if (componentModels is null || !componentModels.Any())
            return Task.FromResult(Results.NotFound());
        
        if (componentModels.Count == 1 ||
            componentRepository.IconExcistOnOtherComponent(componentModels, command.EditedIconData))
            componentRepository.DeleteIconFromFolder(command.EditedComponent.IconData);
            
        command.EditedComponent.IconData = command.EditedIconData;
        for(int i = 0;i<componentModels.Count;i++)
            if (componentModels[i].Id == command.EditedComponent.Id) 
                componentModels[i] =  command.EditedComponent;
        
        componentRepository.WriteIconToFolder(command.EditedIconData);
        return Task.FromResult(componentRepository.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public async Task<ComponentModel[]?> Handle(GetAllCommand command, CancellationToken cancellationToken) =>
        await componentRepository.ReadAllFromJsonFile();
    public async Task<ComponentModel> Handle(GetByIdCommand request, CancellationToken cancellationToken) => 
        await componentRepository.ReadByIdFromJsonFile(request.Id);
}
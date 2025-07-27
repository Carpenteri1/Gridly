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
        var componentModels = 
            componentRepository.Get().Result.ToList();
        
        if (handlerHelper.IconDataHasValue(command.IconData))
        {
            if(!handlerHelper.UploadIcon(command, componentModels))
                return Task.FromResult(Results.NotFound());   
        }
  
        command.ComponentSettings = new ComponentSettingsModel(200, 200);
        componentModels.Add(command);
        
        return Task.FromResult(componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
    {
        var componentModels = 
            componentRepository.Get().Result.ToList();
        
        if (!componentModels.Any())
            return Task.FromResult(Results.NotFound());

        var component = componentModels.First(x => x.Id == command.Id);
        
        if (handlerHelper.IconDataHasValue(component.IconData))
        {
            if(!handlerHelper.DeleteIcon(component, componentModels))
                return Task.FromResult(Results.NotFound());   
        }
        
        componentModels = componentModels.Where(x => x.Id != command.Id).ToList();

        return Task.FromResult(componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(EditComponentCommand command, CancellationToken cancellationToken)
    {
        var componentModels = 
            componentRepository.Get().Result.ToArray();

        if (!componentModels.Any())
            return Task.FromResult(Results.NotFound());  
        
        if (handlerHelper.IconDataHasValue(command.IconData))
        {
            if(!handlerHelper.UploadIcon(command, componentModels)) 
                return Task.FromResult(Results.NotFound());  
            
            handlerHelper.DeleteIcon(componentModels.FirstOrDefault(x => x.Id == command.Id), componentModels);
        }
        
        for(int i = 0;i<componentModels.Length;i++)
            if (componentModels[i].Id == command.Id) 
                componentModels[i] =  command;
        
        return Task.FromResult(componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));
    }
    
    public Task<IResult> Handle(BatchEditComponentCommand commands, CancellationToken cancellationToken)
    {
        var componentModels = componentRepository.Get().Result.ToArray();
        
        if(!componentModels.Any())
            return Task.FromResult(Results.NotFound());  
        
        foreach (var c in commands)
        {
            if (handlerHelper.IconDataHasValue(c.IconData))
            {
                if (!handlerHelper.UploadIcon(c, componentModels))
                    return Task.FromResult(Results.NotFound());  
            }
        }

        for (int i = 0; i < componentModels.ToArray().Length; i++)
        {
            var index = commands.IndexOf(componentModels[i]);
            if (index != i)
            {
                componentModels = commands.ToArray();
            }
            
            var c = commands.Find(x => x.Id == componentModels[i].Id);
            if (c != null && componentModels[i].Id == c.Id)
                componentModels[i] = c;
        }
        
        return Task.FromResult(componentRepository.Save(componentModels) ? 
            Results.Ok() : Results.StatusCode(500));;
    }
    
    public async Task<ComponentModel[]> Handle(GetAllComponentCommand command, CancellationToken cancellationToken) =>
        (await componentRepository.Get()).ToArray();
    public async Task<ComponentModel?> Handle(GetByIdComponentCommands command, CancellationToken cancellationToken) => 
        await componentRepository.GetById(command.Id);
}
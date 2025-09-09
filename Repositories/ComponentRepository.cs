using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository(IDbConnection connection) : IComponentRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    private readonly IFileService _fileService;
    
    public bool Insert(ComponentModel component)
    {
        if (_dbCommandRunner.Insert(QueryStrings.InsertToComponent, component) is not true)
            return false;
        
        if(_dbCommandRunner.Insert(QueryStrings.InsertToComponentSettings, component.ComponentSettings) is not true)
            return false;
        
        if (component.IconData != null && _dbCommandRunner.Insert(QueryStrings.InsertToIcon, component.IconData with {componentId = component.Id}) is not true)
            return false;

        return true;
    }
    
    public bool Insert(IEnumerable<ComponentModel> component)
    {
        return true;
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var builder = new SqlBuilder();
        
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        builder.LeftJoin(QueryStrings.JoinComponentSettings);
        builder.LeftJoin(QueryStrings.JoinIconData);
        
        var componentDtos = 
            await _dbCommandRunner.SelectMany<ComponentDtoModel>(template.RawSql, template.Parameters);

        return componentDtos.Select(dto => new ComponentModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Url = dto.Url,
                IconUrl = dto.IconUrl,
                TitleHidden = dto.TitleHidden,
                ImageHidden = dto.ImageHidden,
                IconData = new IconModel(componentId: dto.Id, dto.IconName, dto.Type, dto.Base64Data),
                ComponentSettings = new ComponentSettingsModel(componentId: dto.Id, width: dto.Width, height: dto.Height)
            });
    }
    
    public async Task<ComponentModel?> GetById(int Id)
    {
        var builder = new SqlBuilder();                                                       
                                                                                       
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery); 
        builder.LeftJoin(QueryStrings.JoinIconData);
        builder.Where(QueryStrings.WhereIconData, new { ComponentId = Id });
        
        return await _dbCommandRunner.Select<ComponentModel>(template.RawSql, template.Parameters);
    }

    public bool Delete(ComponentModel component)
    {
        var builder = new SqlBuilder();                                                       
                                                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteComponentQuery); 
        builder.Where(QueryStrings.WhereComponentSettingsData, new { ComponentId = component.Id });
        if (component.IconData is not null)
        {
            builder.AddTemplate(QueryStrings.DeleteIconQuery);
            builder.Where(QueryStrings.WhereIconData, new { ComponentId = component.Id });
        }
        
        return _dbCommandRunner.Delete(template.RawSql, template.Parameters);   
    }
    
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components)
    {
        var unusedIcons = new List<string>();
        var iconFiles = _fileService.GetAllIcons();
        
        foreach (var icon in iconFiles)
        {
            if (!components.Any(x => x.IconData != null 
                                     && $"{x.IconData.name}.{x.IconData.type}" == icon.Name))
            {
                unusedIcons.Add(icon.Name);
            }
        }
        
        return unusedIcons.Any() ? unusedIcons : new List<string>();
    }
}
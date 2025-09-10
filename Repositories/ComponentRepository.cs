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
    
    public async Task<bool> Insert(ComponentModel component)
    {
        
        if (await _dbCommandRunner.Execute(QueryStrings.InsertToComponentQuery, 
                component) is not true)
            return false;
        
        if(await _dbCommandRunner.Execute(QueryStrings.InsertToComponentSettingsQuery, 
               component.ComponentSettings) is not true)
            return false;
        
        if (component.IconData != null && await _dbCommandRunner.Execute(QueryStrings.InsertToIconQuery,
                component.IconData with {componentId = component.Id}) is not true)
            return false;

        return true;
    }

    public async Task<bool> BatchEdit(IEnumerable<ComponentModel>? components)
    {
        var ctx = await Get();
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentQuery);
        builder.Where(QueryStrings.WhereComponentPrimaryKeyEqualsComponentSettingsForeignKey);
        return await _dbCommandRunner.Execute(template.RawSql, ctx.Except(components));
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var builder = new SqlBuilder();
        
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        builder.LeftJoin(QueryStrings.JoinComponentSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        
        var Dtos = 
            await _dbCommandRunner.SelectMany<ComponentDtoModel>(template.RawSql, template.Parameters);

        return Factories.ComponentFactory.CreateMany(Dtos);
    }
    
    public async Task<ComponentModel?> GetById(int componentId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        builder.LeftJoin(QueryStrings.JoinComponentSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        builder.Where(QueryStrings.WhereIconForeignKeyEqualsComponentPrimaryKey, new {Id = componentId});
        
        var dto = await _dbCommandRunner.Select<ComponentDtoModel>(template.RawSql,template.Parameters);
        return Factories.ComponentFactory.Create(dto);
    }

    public async Task<bool> Delete(ComponentModel component)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromComponentSettingsQuery); 
        builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualComponentObjectId, new { ComponentId = component.Id });
        if (!await _dbCommandRunner.Execute(template.RawSql, template.Parameters))
            return false;
            
        if (component.IconData != null)
        {
            builder = new SqlBuilder();                                                       
            template = builder.AddTemplate(QueryStrings.DeleteFromIconQuery); 
            builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualComponentObjectId, new { ComponentId = component.Id });
            if(!await _dbCommandRunner.Execute(template.RawSql, template.Parameters))
                return false;
        }
        
        builder = new SqlBuilder();
        template = builder.AddTemplate(QueryStrings.DeleteFromComponentQuery);
        builder.Where(QueryStrings.WhereComponentIdPrimaryKeyEqualComponentObjectId, component);
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
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
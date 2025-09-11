using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentSettingsRepository(IDbConnection connection) : IComponentSettingsRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<bool> Insert(ComponentSettingsModel settings)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToComponentSettingsQuery, settings);
    }

    public async Task<bool> Edit(ComponentModel component)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentSettingsQuery);
        builder.Where(QueryStrings.WhereComponentIdPrimaryKeyEqualComponentObjectId, component);
        return await _dbCommandRunner.Execute(template.RawSql,template.Parameters);
    }

    public async Task<bool> BatchEdit(IEnumerable<ComponentModel>? components)
    {
        var ctx = await Get();
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentQuery);
        builder.Where(QueryStrings.WhereComponentPrimaryKeyEqualsComponentSettingsForeignKeyWithAlias);
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
        builder.Where(QueryStrings.WhereIconForeignKeyEqualsComponentPrimaryKeyWithAlias, new {Id = componentId});
        
        var dto = await _dbCommandRunner.Select<ComponentDtoModel>(template.RawSql,template.Parameters);
        return Factories.ComponentFactory.Create(dto);
    }

    public async Task<bool> Delete(int Id)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromComponentSettingsQuery); 
        builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualId, new { ComponentId = Id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
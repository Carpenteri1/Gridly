using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository(IDbConnection connection) : IComponentRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<ComponentModel> Insert(ComponentModel component)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToComponentQuery, component);
    }
    
    
    public async Task<bool> Edit(ComponentModel component)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentQuery);
        builder.Where(QueryStrings.WhereComponentIdPrimaryKeyEqualComponentObjectId, component);
        return await _dbCommandRunner.Execute(template.RawSql,template.Parameters) != null;
    }

    public async Task<bool> BatchEdit(IEnumerable<ComponentModel>? components)
    {
        var ctx = await Get();
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentQuery);
        builder.Where(QueryStrings.WhereComponentForeignKeyEqualsComponentSettingsForeignKeyWithAlias);
        return await _dbCommandRunner.Execute(template.RawSql, ctx.Except(components));
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        
        builder.LeftJoin(QueryStrings.JoinComponentSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconsConnectedDataQuery);
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
        builder.LeftJoin(QueryStrings.JoinIconsConnectedDataQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        builder.Where(QueryStrings.WhereComponentIdEqualsComponentIdWithAlias, new {componentId});
        var dto = await _dbCommandRunner.Select<ComponentDtoModel>(template.RawSql,template.Parameters);
        return Factories.ComponentFactory.Create(dto);
    }

    public async Task<bool> Delete(ComponentModel component)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromComponentQuery);
        builder.Where(QueryStrings.WhereComponentIdPrimaryKeyEqualComponentObjectId, component);
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters) != null;
    }
}
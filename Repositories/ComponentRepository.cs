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
        builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualId, component);
        return await _dbCommandRunner.Execute(template.RawSql,template.Parameters);
    }

    public async Task<bool> BatchEdit(IEnumerable<ComponentModel>? components)
    {
        var parameters = components
            .Select(c => new 
            { 
                c.Id,
                c.IndexPosition,
                Width = c.ComponentSettings.Width,
                Height = c.ComponentSettings.Height
            })
            .ToList();

        var result = await connection.ExecuteAsync(QueryStrings.UpdateBatchComponentQuery, parameters);
        return result > 0;
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        
        builder.LeftJoin(QueryStrings.JoinComponentSettingsQuery);
        builder.LeftJoin(QueryStrings.JoinIconsConnectedDataQuery);
        builder.LeftJoin(QueryStrings.JoinIconDataQuery);
        builder.OrderBy(QueryStrings.IndexPositionWithAlias);
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

    public async Task<bool> Delete(int id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromComponentQuery);
        builder.Where(QueryStrings.WhereIdForeignKeyEqualId, new { Id = id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
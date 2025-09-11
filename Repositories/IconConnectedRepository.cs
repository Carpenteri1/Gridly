using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;

namespace Gridly.Repositories;

public class IconConnectedRepository(IDbConnection connection) : IIconConnectedRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<IconConnectedDtoModel> GetById(int? componentId, int? iconId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconConnectedQuery);
        
        if(componentId != null)
            builder.Where(QueryStrings.WhereIconConnectedComponentIdForeignKeyEqualIdWithAlias, new {ComponentId = componentId});
        if(iconId != null)
            builder.Where(QueryStrings.WhereIconConnectedIconIdForeignKeyEqualWithAlias, new {IconId = iconId});

        var dto = await _dbCommandRunner.Select<IconConnectedDtoModel>(template.RawSql, template.Parameters);
        return dto;        
    }

    public async Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? componentId, int? iconId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconConnectedQuery);
        
        if(componentId != null)
            builder.Where(QueryStrings.WhereIconConnectedComponentIdForeignKeyEqualIdWithAlias, new {ComponentId = componentId});
        if(iconId != null)
            builder.Where(QueryStrings.WhereIconConnectedIconIdForeignKeyEqualWithAlias, new {IconId = iconId});

        return await _dbCommandRunner.SelectMany<IconConnectedDtoModel>(template.RawSql, template.Parameters);
    }

    public async Task<bool> Insert(IconConnectedDtoModel model)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.InsertToConnectedIconQuery);
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }

    public async Task<bool> Delete(int componentId)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.DeleteFromIconsConnectedQuery);
        builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualId, new {ComponentId = componentId});
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
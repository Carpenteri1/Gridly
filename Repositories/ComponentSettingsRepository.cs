using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentSettingsRepository(IDbConnection connection) : IComponentSettingsRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<ComponentSettingsModel> Insert(ComponentSettingsModel settings)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToComponentSettingsQuery, settings);
    }

    public async Task<ComponentSettingsModel> Edit(ComponentSettingsModel settings)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateComponentSettingsQuery);
        builder.Where(QueryStrings.WhereComponentIdPrimaryKeyEqualComponentObjectId, settings);
        return await _dbCommandRunner.Execute(template.RawSql,settings);
    }
    
    public async Task<bool> Delete(int Id)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromComponentSettingsQuery); 
        builder.Where(QueryStrings.WhereComponentIdForeignKeyEqualId, new { ComponentId = Id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters) == null;
    }
}
using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class SettingsRepository(IDbConnection connection) : ISettingsRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<SettingsModel> Insert(SettingsModel settings) => 
        await _dbCommandRunner.Execute(QueryStrings.InsertToSettingsQuery, settings);
    
    public async Task<SettingsModel> Edit(SettingsModel settings)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateSettingsQuery);
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId, settings);
        return await _dbCommandRunner.Execute(template.RawSql,settings);
    }
    
    public async Task<bool> Delete(int Id)
    {
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromSettingsQuery); 
        builder.Where(QueryStrings.WhereCardIdForeignKeyEqualId, new { CardId = Id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
}
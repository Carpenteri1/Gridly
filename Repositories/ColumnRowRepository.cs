using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Repositories;

public class ColumnRowRepository(IDbConnection connection) : IColumnRowRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);

    public async Task<ColumnRowModel> Insert(ColumnRowModel columnRow)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToRowQuery, columnRow);
    }

    public async Task<IEnumerable<ColumnRowModel>?> Get()
    {
        var builder = new SqlBuilder();
        
        var template = builder.AddTemplate(QueryStrings.SelectRowQuery);
        builder.OrderBy(QueryStrings.RowPositionWithAlias);
        var Dtos = 
            await _dbCommandRunner.SelectMany<ColumnRowDtoModel>(template.RawSql, template.Parameters);
        return Factories.ColumnRowFactory.CreateMany(Dtos);
    }

    public async Task<bool> BatchDelete(IEnumerable<ColumnRowModel> columnRows)
    {
        if (columnRows is null)
            return false;
        
        var ids = columnRows.Select(r => r.Id).ToArray();

        var result = await connection.ExecuteAsync(
            QueryStrings.BatchDeleteRowColumnQuery,
            new { Ids = ids });
       
        return result > 0;    
    }
}
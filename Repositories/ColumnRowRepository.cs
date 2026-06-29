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

    public Task<IEnumerable<ColumnRowModel>> BatchEdit(IEnumerable<ColumnRowModel> columnRows)
    {
        //TODO implement query
        throw new NotImplementedException();
    }

    public async Task<bool> Edit(ColumnRowModel columnRow)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateRowQuery,columnRow);
        builder.Where(QueryStrings.WhereIdEqualsId,  new { Id = columnRow.Id });
        return await _dbCommandRunner.Execute(template.RawSql,template.Parameters);
    }
}
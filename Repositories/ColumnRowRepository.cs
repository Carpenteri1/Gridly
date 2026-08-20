using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class ColumnRowRepository(IDbConnection connection, GridlyDbContext dbContext) : IColumnRowRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);

    public async Task<ColumnRowModel> Insert(ColumnRowModel columnRow)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToRowQuery, columnRow);
    }

    public async Task<IEnumerable<ColumnRowModel>?> Get()
    {
        var entities = await dbContext.RowColumns
            .AsNoTracking()
            .OrderBy(r => r.RowPosition)
            .ToListAsync();
        return Factories.ColumnRowFactory.CreateMany(entities);
    }

    public async Task<bool> BatchDelete(IEnumerable<ColumnRowModel> columnRows)
    {
        if (columnRows is null)
            return false;

        var ids = columnRows.Select(r => r.Id).ToArray();

        var result = await dbContext.RowColumns
            .Where(r => ids.Contains(r.Id))
            .ExecuteDeleteAsync();

        return result > 0;
    }
    
    public async Task<bool> BatchEdit(IEnumerable<ColumnRowModel> columnRows)
    {
        if (columnRows is null)
            return false;

        var parameters = columnRows
            .Select(r => new 
            { 
                r.Id,
                r.RowPosition,
                r.RowWidth
            })
            .ToList();

        var result = await connection.ExecuteAsync(QueryStrings.UpdateBatchRowColumnQuery, parameters);
        return result > 0;
    }
}

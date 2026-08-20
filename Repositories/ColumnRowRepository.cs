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

        var rows = columnRows.ToList();
        if (rows.Count == 0)
            return false;

        var ids = rows.Select(r => r.Id).ToList();
        var entities = await dbContext.RowColumns
            .Where(e => ids.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id);

        foreach (var row in rows)
        {
            if (!entities.TryGetValue(row.Id, out var entity))
                continue;

            entity.RowPosition = row.RowPosition;
            entity.RowWidth = row.RowWidth;
        }

        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
}

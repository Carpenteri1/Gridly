using Gridly.Data;
using Gridly.Extension;
using Gridly.Models;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class ColumnRowRepository(GridlyDbContext dbContext) : IColumnRowRepository
{
    public async Task<ColumnRowModel> Insert(ColumnRowModel columnRow)
    {
        var entity = Factories.ColumnRowFactory.Create(columnRow);
        dbContext.RowColumns.Add(entity);
        await dbContext.SaveChangesAsync();
        return Factories.ColumnRowFactory.Create(entity);
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
        => await dbContext.RowColumns
            .SelectMatchingRows(columnRows)
            .ExecuteDeleteAsync() > 0;
    
    public async Task<bool> BatchEdit(IEnumerable<ColumnRowModel> columnRows)
    {
        var entities = await dbContext.RowColumns
            .SelectMatchingRows(columnRows)
            .ToDictionaryAsync(e => e.Id);
        
        foreach (var row in columnRows.ToList())
        {
            if (!entities.TryGetValue(row.Id, out var entity))
                continue;

            entity.RowPosition = row.RowPosition;
            entity.RowWidth = row.RowWidth;
        }

        return await dbContext.SaveChangesAsync() > 0;
    }
}

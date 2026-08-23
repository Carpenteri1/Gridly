using Gridly.Data;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconRepository(GridlyDbContext dbContext) : IIconRepository
{
    public async Task<IconModel> Insert(IconModel icon)
    {
        var entity = new IconEntity
        {
            Name = icon.Name,
            Type = icon.Type,
            Base64Data = icon.Base64Data,
            MaterialIcon = icon.MaterialIcon,
        };
        dbContext.Icons.Add(entity);
        await dbContext.SaveChangesAsync();
        return Factories.IconFactory.Create(entity);
    }
    
    public async Task<bool> Delete(int Id)
    {
        var result = await dbContext.Icons.Where(i => i.Id == Id).ExecuteDeleteAsync();
        return result > 0;
    }
}
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconRepository(GridlyDbContext dbContext) : IIconRepository
{
    public async Task<IconModel> Insert(IconModel icon)
    {
        var entity = Factories.IconFactory.Create(icon);
        dbContext.Icons.Add(entity);
        await dbContext.SaveChangesAsync();
        return Factories.IconFactory.Create(entity);
    }
    
    public async Task<bool> Delete(int id) 
        => await dbContext.Icons.Where(i => i.Id == id).ExecuteDeleteAsync() > 0;
    
}
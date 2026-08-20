using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Factories;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconRepository(IDbConnection connection, IFileService fileService, GridlyDbContext dbContext) : IIconRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);

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
    
    public List<string> FindUnusedIcons(IEnumerable<CardModel> cards)
    {
        var unusedIcons = new List<string>();
        var iconFiles = fileService.GetAllIcons();
        
        foreach (var icon in iconFiles)
        {
            if (!cards.Any(x => x.IconData != null 
                                     && $"{x.IconData.Name}.{x.IconData.Type}" == icon.Name))
            {
                unusedIcons.Add(icon.Name);
            }
        }
        
        return unusedIcons.Any() ? unusedIcons : new List<string>();
    }
}
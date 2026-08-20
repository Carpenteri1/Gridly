using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Factories;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Services;

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

    public async Task<IconModel> Edit(IconModel icon)
    {
        var entity = await dbContext.Icons.FindAsync(icon.Id);
        if (entity is null)
            return icon;

        entity.Name = icon.Name;
        entity.Type = icon.Type;
        entity.Base64Data = icon.Base64Data;
        entity.MaterialIcon = icon.MaterialIcon;

        await dbContext.SaveChangesAsync();
        return IconFactory.Create(entity);
    }

    public async Task<IconModel> GetByFullName(IconModel icon)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconQuery);
        builder.Where(QueryStrings.WhereIconNameEqualsNameWithAlias, icon.Name);
        builder.Where(QueryStrings.WhereIconTypeEqualsTypeWithAlias, icon.Type);
        return await _dbCommandRunner.Select<IconModel>(template.RawSql, icon);
    }

    public async Task<IconModel> GetById(int Id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconQuery);
        builder.Where("i.Id = @Id", new {Id});
        return await _dbCommandRunner.Select<IconModel>(template.RawSql, template.Parameters);
    }

    public async Task<bool> Delete(int Id)
    { 
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromIconQuery); 
        builder.Where(QueryStrings.WhereIdEqualsId, new { Id});
        var s = await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
        return s;
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
using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Models;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class IconRepository(IDbConnection connection, IFileService fileService, GridlyDbContext dbContext) : IIconRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);

    public async Task<IconModel> Insert(IconModel icon)
    {
        return await _dbCommandRunner.Execute(QueryStrings.InsertToIconQuery, icon);
    }

    public async Task<IconModel> Edit(IconModel icon)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.UpdateIconQuery, icon);
        builder.Where(QueryStrings.WhereIdEqualsId, new { Id = icon.Id });
        return await _dbCommandRunner.Execute(template.RawSql, icon);
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
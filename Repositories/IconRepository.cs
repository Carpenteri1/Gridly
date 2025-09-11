using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class IconRepository(IDbConnection connection, IFileService fileService) : IIconRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);
    
    public async Task<bool> Insert(IconModel icon) => 
        await _dbCommandRunner.Execute(QueryStrings.InsertToComponentSettingsQuery, icon);
    
    public async Task<IconModel> Edit(int Id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconQuery);
        builder.Where(QueryStrings.WhereIconIdForeignKeyEqualIdWithAlias, new {IconId =  Id});
        return await _dbCommandRunner.Select<IconModel>(template.RawSql, template.Parameters);
    }

    public async Task<IconModel> GetById(int Id)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(QueryStrings.SelectIconQuery);
        builder.Where(QueryStrings.WhereIconIdForeignKeyEqualIdWithAlias, new {IconId =  Id});
        return await _dbCommandRunner.Select<IconModel>(template.RawSql, template.Parameters);
    }

    public async Task<bool> Delete(int Id)
    { 
        var builder = new SqlBuilder();                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteFromIconQuery); 
        builder.Where(QueryStrings.WhereIconIdForeignKeyEqualId, new {IconId = Id });
        return await _dbCommandRunner.Execute(template.RawSql, template.Parameters);
    }
    

    
    public List<string> FindUnusedIcons(IEnumerable<ComponentModel> components)
    {
        var unusedIcons = new List<string>();
        var iconFiles = fileService.GetAllIcons();
        
        foreach (var icon in iconFiles)
        {
            if (!components.Any(x => x.IconData != null 
                                     && $"{x.IconData.name}.{x.IconData.type}" == icon.Name))
            {
                unusedIcons.Add(icon.Name);
            }
        }
        
        return unusedIcons.Any() ? unusedIcons : new List<string>();
    }
}
using Dapper;
using Gridly.Configuration;
using Gridly.Constants;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ComponentRepository : IComponentRepository
{
    private int? RowsEffected;
    private RepositoryHelper repositoryHelper;
    private readonly IDataConverter<ComponentModel> dataConverter;
    private readonly IFileService fileService;
    
    public bool Insert(ComponentModel component)
    {
        if (component.IconData is not null)
        {
            repositoryHelper.Insert(QueryStrings.InsertToIcon,component.IconData); 
        }
        return repositoryHelper.Insert(QueryStrings.InsertToComponent,component);                   
    }
    
    public bool Insert(IEnumerable<ComponentModel> component)
    {
        return true;
    }

    public async Task<IEnumerable<ComponentModel>?> Get()
    {
        var builder = new SqlBuilder();
        
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery);
        builder.LeftJoin(QueryStrings.LeftJoinIconData);
        
        return await repositoryHelper.SelectMany<ComponentModel>(template.RawSql, template.Parameters);
    }
    
    public async Task<ComponentModel?> GetById(int Id)
    {
        var builder = new SqlBuilder();                                                       
                                                                                       
        var template = builder.AddTemplate(QueryStrings.SelectComponentQuery); 
        builder.LeftJoin(QueryStrings.LeftJoinIconData);
        builder.Where(QueryStrings.WhereIconData, new { ComponentId = Id });
        
        return await repositoryHelper.Select<ComponentModel>(template.RawSql, template.Parameters);
    }

    public bool Delete(ComponentModel component)
    {
        var builder = new SqlBuilder();                                                       
                                                                                       
        var template = builder.AddTemplate(QueryStrings.DeleteComponentQuery); 
        builder.Where(QueryStrings.WhereComponentData, new { ComponentId = component.Id });
        if (component.IconData is not null)
        {
            builder.AddTemplate(QueryStrings.DeleteIconQuery);
            builder.Where(QueryStrings.WhereIconData, new { ComponentId = component.Id });
        }
        
        return repositoryHelper.Delete(template.RawSql, template.Parameters);   
    }
    
    
    //TODO move these below here
    public bool UploadIcon(IconModel iconData)
    {
        string filePath = FilePaths.IconPath + $"{iconData.name}.{iconData.type}";
        return fileService.WriteAllBitesToFile(filePath, iconData.base64Data);
    }
    
    public bool DeleteIcon(string name, string type)
    {
        string filePath = FilePaths.IconPath + $"{name}.{type}";
        return fileService.FileExist(filePath) && fileService.DeletedFile(filePath);
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
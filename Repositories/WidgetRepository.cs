using System.Data;
using Dapper;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Repositories;

public class WidgetRepository(IDbConnection connection) : IWidgetRepository
{
    private DbCommandRunner _dbCommandRunner = new (connection);

    public async Task<IEnumerable<WidgetModel>> Get()
    {
        var builder = new SqlBuilder();
        
        var template = builder.AddTemplate(QueryStrings.SelectWidgetQuery);
        builder.LeftJoin(QueryStrings.JoinWidgetType);
        var Dtos = 
            await _dbCommandRunner.SelectMany<WidgetDtoModel>(template.RawSql, template.Parameters);
        return Factories.WidgetFactory.CreateMany(Dtos);
    }
}

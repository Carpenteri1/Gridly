using System.Data;
using Dapper;
using Gridly.Models;

namespace Gridly.Repositories;

public class VersionRepository : IVersionRepository
{
    private readonly IDbConnection _connection;

    public VersionRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public (bool, VersionModel) GetVersion()
    {
        _connection.Open();
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(@"SELECT * FROM Version/**where**/"); 
        var localVersion = _connection.QueryFirstOrDefault<VersionModel>(template.RawSql, template.Parameters);
        _connection.Close();
        return (localVersion != null,localVersion);
    }

    public (bool, VersionModel) SaveVersion(VersionModel version)
    {
        _connection.Open();
        var builder = new SqlBuilder();
        var template = builder.AddTemplate("INSERT INTO Version (Name, NewRelease) VALUES (@Name, @NewRelease)");
        int rowsAffected = _connection.Execute(template.RawSql, version);
        _connection.Close();
        return (rowsAffected != 0, version);
    }
}
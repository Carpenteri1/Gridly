using System.Data;
using Dapper;

namespace Gridly.Data;

public class DbCommandRunner
{
    private readonly IDbConnection _connection;                  
    private int _rowsEffected;                  
                                                             
    public DbCommandRunner(IDbConnection connection)         
    {                                                            
        _connection = connection;                                
    }                                                            
    
    public async Task<bool> Execute<T>(string query,IEnumerable<T> parameters)
    {
        _connection.Open();
        _rowsEffected = await _connection.ExecuteAsync(query, parameters);
        _connection.Close();
        return _rowsEffected > 0;
    }
    
    public async Task<bool> Execute<T>(string query,T parameters)
    {
        _connection.Open();
        _rowsEffected = await _connection.ExecuteAsync( query, parameters);
        _connection.Close();
        return _rowsEffected > 0;
    }
    
    public async Task<IEnumerable<T>> SelectMany<T>(string query,object parameters)
    {
        _connection.Open();                                             
        var items = await _connection.QueryAsync<T>(query, parameters);
        _connection.Close();
        return items;
    }
    
    public async Task<T> Select<T>(string query,object parameters)
    {
        _connection.Open();                                             
        var items = await _connection.QueryFirstAsync<T>(query, parameters);
        _connection.Close();
        return items;
    }
}
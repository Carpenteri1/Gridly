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
    
    public bool Insert<T>(string query,T item)
    {
        _connection.Open();
        _rowsEffected = _connection.Execute( query, item);
        _connection.Close();
        return _rowsEffected > 0;
    }
    
    public async Task<IEnumerable<T>> SelectMany<T>(string query,object parameter)
    {
        _connection.Open();                                             
        var items = await _connection.QueryAsync<T>(query);
        _connection.Close();
        return items;
    }
    
    public async Task<T> Select<T>(string query,object item)
    {
        _connection.Open();                                             
        var items = await _connection.QueryFirstAsync<T>(query, item);
        _connection.Close();
        return items;
    }
    
    public bool Delete<T>(string query,T item)
    {
        _connection.Open(); 
        _rowsEffected = _connection.Execute(query, item);
        _connection.Close();
        return _rowsEffected > 0;
    }
}
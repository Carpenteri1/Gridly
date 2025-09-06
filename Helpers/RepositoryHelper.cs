using System.Data;
using Dapper;

namespace Gridly.helpers;

public class RepositoryHelper
{
    private RepositoryHelper repositoryHelper;                   
    private readonly IDbConnection _connection;                  
                                                             
    public RepositoryHelper(IDbConnection connection)         
    {                                                            
        _connection = connection;                                
    }                                                            
    
    public bool Insert<T>(string query,T item)
    {
        int? rowsEffected;
        _connection.Open();
        rowsEffected = _connection.Execute(query, item);     
        _connection.Close();
        return rowsEffected > 0;
    }
    
    public async Task<IEnumerable<T>> SelectMany<T>(string query,object parameter)
    {
        _connection.Open();                                             
        var items = await _connection.QueryAsync<T>(query, parameter);
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
        int? rowsEffected;
        rowsEffected = _connection.Execute(query, item);
        _connection.Close();
        return rowsEffected > 0;
    }
}
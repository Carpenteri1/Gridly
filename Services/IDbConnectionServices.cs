using System.Data;
namespace Gridly.Data;

public interface IDbConnectionServices
{
    public IDbConnection CreateConnection();
}

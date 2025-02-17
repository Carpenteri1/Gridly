using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class GetComponentHandler
{
    public static async Task<ComponentModel[]?> Get() => 
        await DataStorage.ReadAllFromJsonFile();
    
    public static async Task<ComponentModel?> GetById(int Id) => 
        await DataStorage.ReadByIdFromJsonFile(Id);
}
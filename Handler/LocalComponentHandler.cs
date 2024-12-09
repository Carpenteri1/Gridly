using System.Text.Json;
using Gridly.Services;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    public static IResult Save(dynamic newComponent) =>
        DataStorage.ReadToJsonFile(newComponent) ? 
            Results.Ok() : Results.StatusCode(500);

    public static async Task<dynamic[]> Get() => 
        await DataStorage.ReadFromJsonFile();

    public static async Task<IResult> Delete(int componentId)
    {
       var components = await DataStorage.ReadFromJsonFile();
       return DataStorage.ReadToJsonFile(components.Where(x => x.id != componentId).ToArray())
           ? Results.Ok()
           : Results.NotFound();
    }
}
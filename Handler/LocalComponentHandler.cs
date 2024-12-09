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
        //TODO get the id in the list here and make a new list without that id and store it
        var components = await DataStorage.ReadFromJsonFile();
        //var component = components.FirstOrDefault(c => c.GetProperty("Id").GetInt32() == componentId);
       return Results.Ok();
    }
}
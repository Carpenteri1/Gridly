using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    public static IResult Save(ComponentModel newComponent) =>
        DataStorage.ReadToJsonFile(newComponent) ? 
            Results.Ok() : Results.StatusCode(500);

    public static async Task<ComponentModel[]?> Get() => 
        await DataStorage.ReadFromJsonFile();

    public static IResult Delete(int componentId)
    { 
       var componentModel  = DataStorage.ReadFromJsonFile()
           .Result?.Where(x => x.Id != componentId).FirstOrDefault();
       
       if (componentModel != null) 
           return DataStorage.ReadToJsonFile(componentModel) 
               ? Results.Ok() : Results.StatusCode(500);
       
       return !DataStorage.ReadToJsonFile(componentModel) ? 
           Results.StatusCode(500) : Results.Ok();
    }
}
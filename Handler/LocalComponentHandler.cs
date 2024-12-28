using Gridly.Models;
using Gridly.Services;

namespace Gridly.Handler;

public class LocalComponentHandler
{
    public static IResult Save(ComponentModel newComponent)
    {
        var componentModels = DataStorage.ReadFromJsonFile().Result?.ToList();
        componentModels.Add(newComponent);
        
      return DataStorage.ReadToJsonFile(componentModels) ? 
            Results.Ok() : Results.StatusCode(500);

    }

    public static async Task<ComponentModel[]> Get() => 
        await DataStorage.ReadFromJsonFile();

    public static IResult Delete(int componentId)
    { 
       var componentModels  = DataStorage.ReadFromJsonFile()
           .Result?.Where(x => x.Id != componentId).ToList();
       
       if (componentModels.Any()) 
           return DataStorage.ReadToJsonFile(componentModels) 
               ? Results.Ok() : Results.StatusCode(500);
       
       return !DataStorage.ReadToJsonFile(componentModels) ? 
           Results.StatusCode(500) : Results.Ok();
    }
}
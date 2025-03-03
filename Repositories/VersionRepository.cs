using Gridly.Configuration;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class VersionRepository(IDataConverter<VersionModel> dataConverter, IFileService fileService, IVersionEndPoint versionEndPoint) : IVersionRepository
{
    public async Task<VersionModel> GetVersionAsync()
    {
        var (success,model) = await versionEndPoint.GetLatestVersion();
        if (success)
        {
            var jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.RateLimitFilePath);
            if (string.IsNullOrEmpty(jsonFileString) || jsonFileString.Equals("{}"))
            {
                model.CreatedAt = DateTime.Now;
                string jsonString = dataConverter.SerializerToJsonString(model);
                fileService.WriteToJson(FilePaths.RateLimitFilePath, jsonString);
                return model;
            }

            var storedModel = dataConverter.DeserializeJsonString(jsonFileString);
            if (dataConverter.ToInt(storedModel.Name) > dataConverter.ToInt(model.Name))
            {
                model.CreatedAt = DateTime.Now;
                string jsonString = dataConverter.SerializerToJsonString(model);
                fileService.WriteToJson(FilePaths.RateLimitFilePath, jsonString);
                return model;
            }
        }
        return model;
    }
}
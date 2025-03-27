using Gridly.Configuration;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class VersionRepository(IDataConverter<VersionModel> dataConverter, IFileService fileService, IVersionEndPoint versionEndPoint) : IVersionRepository
{
    public async Task<VersionModel> GetLatestVersionAsync()
    {
        var (success,model) = await versionEndPoint.GetLatestVersion();
        if (success)
        {
            var jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.VersionFilePath);
            if (string.IsNullOrEmpty(jsonFileString) || jsonFileString.Equals("{}"))
            {
                string jsonString = dataConverter.SerializerToJsonString(model);
                fileService.WriteToFile(FilePaths.VersionFilePath, jsonString);
                return model;
            }

            var storedModel = dataConverter.DeserializeJson(jsonFileString);
            model.NewRelease = dataConverter.ToInt(model.Name) > dataConverter.ToInt(storedModel.Name);
            if (model.NewRelease)
            {
                string jsonString = dataConverter.SerializerToJsonString(model);
                fileService.WriteToFile(FilePaths.VersionFilePath, jsonString);
                return model;
            }
        }
        return model;
    }
}
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
                fileService.WriteToJson(FilePaths.VersionFilePath, jsonString);
                return model;
            }

            var storedModel = dataConverter.DeserializeJsonString(jsonFileString);
            storedModel.NewRelease = dataConverter.ToInt(storedModel.Name) > dataConverter.ToInt(model.Name);
            if (storedModel.NewRelease)
            {
                string jsonString = dataConverter.SerializerToJsonString(model);
                fileService.WriteToJson(FilePaths.VersionFilePath, jsonString);
                return model;
            }
        }
        return model;
    }
}
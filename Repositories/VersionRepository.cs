using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class VersionRepository(IDataConverter<VersionModel> dataConverter, IFileService fileService, IVersionEndPoint versionEndPoint) : IVersionRepository
{
    private const string JsonComponentFileName = "rateLimiterData.json";
    private readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(),"Assets/RateLimiterData", JsonComponentFileName);
    public async Task<VersionModel> GetVersionAsync()
    {
        var (success,model) = await versionEndPoint.GetLatestVersion();
        
        if (fileService.FileExcist(FilePath) && success)
        {
            var jsonFile = await fileService.ReadAllFromFileAsync(FilePath);
            if (!string.IsNullOrEmpty(jsonFile))
            {
                var localVersionModel = dataConverter.DeserializeJsonString(jsonFile);
                localVersionModel.newRelease = int.Parse(localVersionModel.tag_name.Trim('v').Trim('.')) <
                                               int.Parse(model.tag_name.Trim('v').Trim('.'));
            }
        }
        else
        {
            string jsonString = dataConverter.SerializerToJsonString(model);
            fileService.WriteToJson(FilePath, jsonString);
            return model;
        }

        return model;
    }
}
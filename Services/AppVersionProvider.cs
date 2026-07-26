using Gridly.Configuration;
using Gridly.Dtos;

namespace Gridly.Services;

public class AppVersionProvider(
    IFileService fileService,
    IDataConverter<AppVersionDtoModel> dataConverter) : IAppVersionProvider
{
    public const string DevSentinelVersion = "0.0.0";

    public async Task<string> GetCurrentVersionAsync()
    {
        if (!fileService.FileExist(FilePaths.VersionDataPath))
            return DevSentinelVersion;

        try
        {
            var content = await fileService.ReadAllFromFileAsync(FilePaths.VersionDataPath);
            var data = dataConverter.DeserializeJson(content);
            return data?.Version ?? DevSentinelVersion;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return DevSentinelVersion;
        }
    }
}

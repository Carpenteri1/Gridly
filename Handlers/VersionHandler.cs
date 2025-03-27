using Gridly.Command;
using Gridly.Configuration;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class VersionHandler(
    IVersionRepository versionRepository, 
    IFileService fileService, 
    IDataConverter<VersionModel> dataConverter) : 
    IRequestHandler<GetAllLatestVersionCommand,VersionModel>,
    IRequestHandler<GetAllCurrentVersionCommand,VersionModel>
{
    public async Task<VersionModel> Handle(GetAllLatestVersionCommand request, CancellationToken cancellationToken)
    {
        var version = await versionRepository.GetLatestVersionAsync();
        return version;
    }
    public async Task<VersionModel> Handle(GetAllCurrentVersionCommand request, CancellationToken cancellationToken)
    {
        var jsonFileString = "{}";
        if (!fileService.FileExist(FilePaths.VersionFilePath))
        {
            fileService.WriteToFile(FilePaths.VersionFilePath,jsonFileString);
            return dataConverter.DeserializeJson(jsonFileString);
        }
        
        jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.VersionFilePath);
        
        return !string.IsNullOrEmpty(jsonFileString) || !jsonFileString.Equals("{}") ?
            dataConverter.DeserializeJson(jsonFileString) : 
            new VersionModel{Name = "", NewRelease = false};
    }
}
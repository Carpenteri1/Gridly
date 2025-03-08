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
        if (fileService.FileExcist(FilePaths.VersionFilePath))
        {
            var jsonFileString = await fileService.ReadAllFromFileAsync(FilePaths.VersionFilePath);
            if (!string.IsNullOrEmpty(jsonFileString) || !jsonFileString.Equals("{}"))
            {
                return dataConverter.DeserializeJsonString(jsonFileString);
            }
        }
        return new VersionModel{Name = "", NewRelease = false};
    }
}
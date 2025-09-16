using Gridly.Command;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class VersionHandler(
    IVersionEndPoint versionEndPoint,
    IMemoryCashingService memoryCashingService) : 
    IRequestHandler<GetVersionCommand, IResult>,
    IRequestHandler<GetLatestVersionCommand, IResult>
{
    public async Task<IResult> Handle(GetVersionCommand request, CancellationToken cancellationToken)
    {
        var cashedVersion = memoryCashingService.Get<VersionModel>("version");
        if (cashedVersion == null)
        {
            var (success, version) = await versionEndPoint.GetVersion();
            if(success)
                memoryCashingService.Store("version", version);
            return success ? Results.Ok(version) : Results.NotFound();
        }

        return cashedVersion != null ? Results.Ok(cashedVersion) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetLatestVersionCommand request, CancellationToken cancellationToken)
    {
        var (success, remoteVersion) = await versionEndPoint.GetLatestVersion();
        if(success)
            memoryCashingService.Store<VersionModel>("version", remoteVersion);
        
        return success != null ? Results.Ok(remoteVersion) : Results.NotFound();
    }
}
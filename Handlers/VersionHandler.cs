using Gridly.Command;
using Gridly.EndPoints;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class VersionHandler(
    IVersionRepository versionRepository,
    IVersionEndPoint versionEndPoint) : 
    IRequestHandler<GetLatestVersionCommand, IResult>,
    IRequestHandler<GetCurrentVersionCommand, IResult>,
    IRequestHandler<SaveVersionCommand, IResult>
{
    public async Task<IResult> Handle(GetLatestVersionCommand request, CancellationToken cancellationToken)
    {
        var (remoteSuccess, remoteVersion) = await versionEndPoint.GetLatestVersion();
        var (localSuccess, localVersion) = versionRepository.GetVersion();
        
        if (remoteSuccess && localSuccess)
        {
            if (!remoteVersion.Name.Equals(localVersion.Name, 
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return Results.Ok(remoteVersion);
            }
            return Results.Ok(localVersion);
        }

        if (!remoteSuccess && localSuccess)
        {
            return Results.Ok(localVersion);
        }

        if (remoteSuccess && !localSuccess)
        {
            remoteVersion.NewRelease = true;
            return Results.Ok(remoteVersion);
        }
        
        return Results.NotFound();
    }
    public Task<IResult> Handle(GetCurrentVersionCommand request, CancellationToken cancellationToken)
    {
        var (success,version) = versionRepository.GetVersion();
        return Task.FromResult(success ? Results.Ok(version) : Results.NotFound());
    }

    public Task<IResult> Handle(SaveVersionCommand command, CancellationToken cancellationToken)
    {
        var (success,version) = versionRepository.SaveVersion(command);
        return Task.FromResult(success ? Results.Ok(version) : Results.StatusCode(500));
    }
}
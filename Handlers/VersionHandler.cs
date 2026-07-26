using Gridly.Constants;
using Gridly.Querys;
using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class VersionHandler(
    IVersionEndPoint versionEndPoint,
    IMemoryCashingService memoryCashingService,
    IAppVersionProvider appVersionProvider) :
    IRequestHandler<GetVersionQuery, IResult>,
    IRequestHandler<GetLatestVersionQuery, IResult>
{
    private static readonly TimeSpan CheckInterval = new VersionRateLimiterModel().Window;

    public async Task<IResult> Handle(GetVersionQuery query, CancellationToken cancellationToken)
    {
        var state = memoryCashingService.Get<VersionCheckStateModel>(CacheKeyStrings.VersionCacheKey);
        return state != null ? Results.Ok(state.Version) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetLatestVersionQuery query, CancellationToken cancellationToken)
    {
        var state = memoryCashingService.Get<VersionCheckStateModel>(CacheKeyStrings.VersionCacheKey);
        if (state != null && DateTime.UtcNow - state.CheckedAtUtc < CheckInterval)
            return Results.Ok(state.Version);

        var (success, remoteVersion) = await versionEndPoint.GetLatestVersion();
        if (!success)
            return state != null ? Results.Ok(state.Version) : Results.NotFound();

        var currentVersion = await appVersionProvider.GetCurrentVersionAsync();
        remoteVersion!.NewRelease = HasNewRelease(currentVersion, remoteVersion.Name);

        var newState = new VersionCheckStateModel { Version = remoteVersion, CheckedAtUtc = DateTime.UtcNow };
        memoryCashingService.Store(CacheKeyStrings.VersionCacheKey, newState, CheckInterval);

        return Results.Ok(remoteVersion);
    }

    private static bool HasNewRelease(string currentVersion, string latestVersion)
    {
        if (string.Equals(currentVersion, AppVersionProvider.DevSentinelVersion, StringComparison.OrdinalIgnoreCase))
            return false;

        return !string.Equals(Normalize(currentVersion), Normalize(latestVersion), StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string version) =>
        version?.TrimStart('v', 'V').Trim() ?? string.Empty;
}

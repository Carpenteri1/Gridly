using Gridly.Commands;
using Gridly.EndPoints;
using Gridly.EndPoints.Interfaces;
using Gridly.Enums;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class RemoteProvidersHandler(
    ILocalProvidersRepository localProvidersRepository,
    IProviderKeysProtectionService providerKeysProtectionService,
    IProvidersEndPoint providersEndPoint) :
    IRequestHandler<SaveProviderKeyCommand, IResult>,
    IRequestHandler<GetRemoteProviderKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(SaveProviderKeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RawKey)) return Results.BadRequest();

        var encrypted = providerKeysProtectionService.Protect(command.RawKey.Trim());
        var success = await localProvidersRepository.Upsert(
            command.Provider,
            encrypted,
            nameof(ProvidersKeyStatusEnum.Unknown));

        return success ? Results.Ok() : Results.BadRequest();
    }

    public async Task<IResult> Handle(GetRemoteProviderKeyStatusQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Provider)) return Results.BadRequest();
        
        var storedKey = await localProvidersRepository.Get(query.Provider);
        if (storedKey is null) return Results.Ok(new ProviderKeyStatusModel { Exists = false, KeyStatus = ProvidersKeyStatusEnum.Unknown });
        
        var rawKey = providerKeysProtectionService.Unprotect(storedKey.EncryptedKey);
        
        var fetchStatus = await providersEndPoint.Validate(rawKey);
        switch (fetchStatus)
        {
            case StatusCodes.Status401Unauthorized:
                return Results.Unauthorized();
            case StatusCodes.Status403Forbidden:
                return Results.Forbid();
            case < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices:
                return Results.BadRequest();
        }

        var status = fetchStatus is StatusCodes.Status200OK
            ? ProvidersKeyStatusEnum.Valid
            : ProvidersKeyStatusEnum.Invalid;

        await localProvidersRepository.UpdateStatus(query.Provider, status.ToString());
        return Results.Ok(new ProviderKeyStatusModel { Exists = true, KeyStatus = status });
    }
}

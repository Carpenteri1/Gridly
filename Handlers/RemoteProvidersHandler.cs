using Gridly.Commands;
using Gridly.Data;
using Gridly.EndPoints.Interfaces;
using Gridly.Entities;
using Gridly.Enums;
using Gridly.Extension;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class RemoteProvidersHandler(
    GridlyDbContext dbContext,
    IProviderKeysProtectionService providerKeysProtectionService,
    IProvidersEndPoint providersEndPoint) :
    IRequestHandler<SaveProviderKeyCommand, IResult>,
    IRequestHandler<GetRemoteProviderKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(SaveProviderKeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RawKey)) return Results.BadRequest();

        var encryptedKey = providerKeysProtectionService.Protect(command.RawKey.Trim());
        var storedKeyEntity = await dbContext.ProviderKeys.GetLocalProviderKeyByProvider(command.Provider);
       
        if (storedKeyEntity is null)
        {
            storedKeyEntity = new ProviderKeyEntity
            {
                Provider = command.Provider,
                EncryptedKey = encryptedKey,
                Status = nameof(ProvidersKeyStatusEnum.Unknown),
                LastValidatedAt = DateTime.UtcNow,
            };
            dbContext.ProviderKeys.Add(storedKeyEntity);
        }
        else
        {
            storedKeyEntity.EncryptedKey = encryptedKey;
            storedKeyEntity.Status = nameof(ProvidersKeyStatusEnum.Unknown);
            storedKeyEntity.LastValidatedAt = DateTime.UtcNow;
        }
        
        var affected = await dbContext.SaveChangesAsync();

        return affected > 0 ? Results.Ok() : Results.BadRequest();
    }

    public async Task<IResult> Handle(GetRemoteProviderKeyStatusQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Provider)) return Results.BadRequest();
        
        var storedKeyEntity = await dbContext.ProviderKeys.GetLocalProviderKeyByProvider(query.Provider);
        if (storedKeyEntity is null) return Results.Ok(new ProviderKeyStatusModel { Exists = false, KeyStatus = ProvidersKeyStatusEnum.Unknown });
        
        var rawKey = providerKeysProtectionService.Unprotect(storedKeyEntity.EncryptedKey!);
        
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

        storedKeyEntity.Status = status.ToString();
        storedKeyEntity.LastValidatedAt = DateTime.UtcNow;

        var affected = await dbContext.SaveChangesAsync(cancellationToken);
        
        return affected > 0 ?  Results.Ok(new ProviderKeyStatusModel { Exists = true, KeyStatus = status }) : Results.BadRequest();
    }
}

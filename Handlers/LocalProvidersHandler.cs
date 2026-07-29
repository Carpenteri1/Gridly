using Gridly.Enums;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class LocalProvidersHandler(ILocalProvidersRepository localProvidersRepository):
    IRequestHandler<GetLocalProviderKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(GetLocalProviderKeyStatusQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Provider)) return Results.BadRequest();
        
        var storedKey = await localProvidersRepository.Get(query.Provider);
        if (storedKey is null) return Results.Ok(new ProviderKeyStatusModel { Exists = false, KeyStatus = ProvidersKeyStatusEnum.Unknown });

        var status = storedKey.Status is nameof(ProvidersKeyStatusEnum.Valid)
            ? ProvidersKeyStatusEnum.Valid
            : ProvidersKeyStatusEnum.Invalid;

        await localProvidersRepository.UpdateStatus(query.Provider, status.ToString());
        return Results.Ok(new ProviderKeyStatusModel { Exists = true, KeyStatus = status });
    }
}

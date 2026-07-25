using Gridly.Commands;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class ProviderKeyHandler(
    IProviderKeysRepository providerKeysRepository,
    IProviderKeysProtectionService providerKeysProtectionService) :
    IRequestHandler<SaveProviderKeyCommand, IResult>,
    IRequestHandler<GetProviderKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(SaveProviderKeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RawKey)) return Results.BadRequest();

        var encrypted = providerKeysProtectionService.Protect(command.RawKey);
        var success = await providerKeysRepository.Upsert(command.Provider, encrypted, nameof(ProviderKeyStatusEnum.Unknown));
        return success ? Results.Ok() : Results.BadRequest();
    }

    public async Task<IResult> Handle(GetProviderKeyStatusQuery query, CancellationToken cancellationToken)
    {
        var apiKey = await providerKeysRepository.Get(query.Provider);
        if (apiKey is null) return Results.Ok(new { exists = false, status = nameof(ProviderKeyStatusEnum.Unknown) });

        var model = ProviderKeyFactory.Create(apiKey);
        return Results.Ok(new { exists = true, status = model.Status });
    }
}

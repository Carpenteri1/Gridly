using Gridly.Commands;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class ApiKeyHandler(
    IApiKeyRepository apiKeyRepository,
    IApiKeyProtectionService apiKeyProtectionService) :
    IRequestHandler<SaveApiKeyCommand, IResult>,
    IRequestHandler<GetApiKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(SaveApiKeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RawKey)) return Results.BadRequest();

        var encrypted = apiKeyProtectionService.Protect(command.RawKey);
        var success = await apiKeyRepository.Upsert(command.Provider, encrypted, ApiKeyStatus.Unknown);
        return success ? Results.Ok() : Results.BadRequest();
    }

    public async Task<IResult> Handle(GetApiKeyStatusQuery query, CancellationToken cancellationToken)
    {
        var apiKey = await apiKeyRepository.Get(query.Provider);
        if (apiKey is null) return Results.Ok(new { exists = false, status = ApiKeyStatus.Unknown });

        var model = ApiKeyFactory.Create(apiKey);
        return Results.Ok(new { exists = true, status = model.Status });
    }
}

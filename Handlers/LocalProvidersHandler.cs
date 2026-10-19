using Gridly.Data;
using Gridly.Enums;
using Gridly.Models;
using Gridly.Querys;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;

public class LocalProvidersHandler(GridlyDbContext dbContext):
    IRequestHandler<GetLocalProviderKeyStatusQuery, IResult>
{
    public async Task<IResult> Handle(GetLocalProviderKeyStatusQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Provider)) return Results.BadRequest();

        var storedKey = await dbContext.ProviderKeys
            .FirstOrDefaultAsync(key => key.Provider == query.Provider, cancellationToken);
        if (storedKey is null) return Results.Ok(new ProviderKeyStatusModel { Exists = false, KeyStatus = ProvidersKeyStatusEnum.Unknown });

        var status = storedKey.Status is nameof(ProvidersKeyStatusEnum.Valid)
            ? ProvidersKeyStatusEnum.Valid
            : ProvidersKeyStatusEnum.Invalid;

        storedKey.Status = status.ToString();
        storedKey.LastValidatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(new ProviderKeyStatusModel { Exists = true, KeyStatus = status });
    }
}

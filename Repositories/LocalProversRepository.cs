using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class LocalProversRepository(IDbConnection connection, GridlyDbContext dbContext) : ILocalProvidersRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<ProviderKeyDtoModel?> Get(string provider)
    {
        var entity = await dbContext.ProviderKeys.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Provider == provider);

        return entity is null
            ? null
            : new ProviderKeyDtoModel
            {
                Id = entity.Id,
                Provider = entity.Provider!,
                EncryptedKey = entity.EncryptedKey!,
                Status = entity.Status!,
                LastValidatedAt = entity.LastValidatedAt,
            };
    }

    public async Task<bool> Upsert(string provider, string encryptedKey, string status)
    {
        object parameters = new
        {
            Provider = provider,
            EncryptedKey = encryptedKey,
            Status = status,
            LastValidatedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpsertProviderKeyQuery, parameters);
    }

    public async Task<bool> UpdateStatus(string provider, string status)
    {
        object parameters = new
        {
            Provider = provider,
            Status = status,
            LastValidatedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpdateProviderKeyStatusQuery, parameters);
    }
}

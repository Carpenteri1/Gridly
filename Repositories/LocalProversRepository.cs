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

    public async Task<ProviderKeyDtoModel?> Get(string provider) =>
        await _dbCommandRunner.Select<ProviderKeyDtoModel>(
            QueryStrings.SelectProviderKeyQuery, new { Provider = provider });

    public async Task<bool> Upsert(string provider, string encryptedKey, string status)
    {
        var entity = await dbContext.ProviderKeys.FirstOrDefaultAsync(p => p.Provider == provider);
        if (entity is not null)
        {
            entity.EncryptedKey = encryptedKey;
            entity.Status = status;
            entity.LastValidatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new ProviderKeyEntity
            {
                Provider = provider,
                EncryptedKey = encryptedKey,
                Status = status,
                LastValidatedAt = DateTime.UtcNow,
            };
            dbContext.ProviderKeys.Add(entity);
        }

        var affected = await dbContext.SaveChangesAsync();
        return affected > 0;
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

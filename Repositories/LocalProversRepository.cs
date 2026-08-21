using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class LocalProversRepository(GridlyDbContext dbContext) : ILocalProvidersRepository
{
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
        var entity = await dbContext.ProviderKeys.FirstOrDefaultAsync(p => p.Provider == provider);
        if (entity is null)
            return false;

        entity.Status = status;
        entity.LastValidatedAt = DateTime.UtcNow;

        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
}

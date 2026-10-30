using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Extension;

public static class LocalProviderQueryExtensions
{
    public static async Task<ProviderKeyEntity?> GetLocalProviderKeyByProvider(
        this IQueryable<ProviderKeyEntity> entities, string provider)
    {
        return await entities
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Provider == provider);
    }
}
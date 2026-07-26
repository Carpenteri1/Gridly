using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;

namespace Gridly.Repositories;

public class ProversRepository(IDbConnection connection) : IProvidersRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<ProviderKeyDtoModel?> Get(string provider) =>
        await _dbCommandRunner.Select<ProviderKeyDtoModel>(
            QueryStrings.SelectProviderKeyQuery, new { Provider = provider });

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

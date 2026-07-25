using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;

namespace Gridly.Repositories;

public class ApiKeyRepository(IDbConnection connection) : IApiKeyRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<ApiKeyDtoModel?> Get(string provider) =>
        await _dbCommandRunner.Select<ApiKeyDtoModel>(
            QueryStrings.SelectThirdPartyApiKeyQuery, new { Provider = provider });

    public async Task<bool> Upsert(string provider, string encryptedKey, string status)
    {
        object parameters = new
        {
            Provider = provider,
            EncryptedKey = encryptedKey,
            Status = status,
            LastValidatedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpsertThirdPartyApiKeyQuery, parameters);
    }

    public async Task<bool> UpdateStatus(string provider, string status)
    {
        object parameters = new
        {
            Provider = provider,
            Status = status,
            LastValidatedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpdateThirdPartyApiKeyStatusQuery, parameters);
    }
}

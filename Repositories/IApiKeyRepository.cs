using Gridly.Dtos;

namespace Gridly.Repositories;

public interface IApiKeyRepository
{
    public Task<ApiKeyDtoModel?> Get(string provider);
    public Task<bool> Upsert(string provider, string encryptedKey, string status);
    public Task<bool> UpdateStatus(string provider, string status);
}

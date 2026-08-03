using Gridly.Dtos;

namespace Gridly.Repositories;

public interface ILocalProvidersRepository
{
    public Task<ProviderKeyDtoModel?> Get(string provider);
    public Task<bool> Upsert(string provider, string encryptedKey, string status);
    public Task<bool> UpdateStatus(string provider, string status);
}

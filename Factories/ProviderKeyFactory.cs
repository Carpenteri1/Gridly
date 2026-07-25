using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class ProviderKeyFactory
{
    public static ProviderKeyModel Create(ProviderKeyDtoModel dto)
        => new()
        {
            Id = dto.Id,
            Provider = dto.Provider,
            Status = dto.Status,
            LastValidatedAt = dto.LastValidatedAt
        };
}

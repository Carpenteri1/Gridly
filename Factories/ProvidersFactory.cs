using Gridly.Dtos;
using Gridly.Enums;
using Gridly.Models;

namespace Gridly.Factories;

public static class ProvidersFactory
{
    public static ProviderKeyModel Create(ProviderKeyDtoModel dto)
        => new()
        {
            Id = dto.Id,
            Provider = dto.Provider,
            KeyStatus = ToStatus(dto.Status),
            LastValidatedAt = dto.LastValidatedAt
        };

    private static ProvidersKeyStatusEnum ToStatus(string status) =>
        Enum.TryParse<ProvidersKeyStatusEnum>(status, ignoreCase: true, out var parsed)
            ? parsed
            : ProvidersKeyStatusEnum.Unknown;
}

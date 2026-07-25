using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class ApiKeyFactory
{
    public static ApiKeyModel Create(ApiKeyDtoModel dto)
        => new()
        {
            Id = dto.Id,
            Provider = dto.Provider,
            Status = dto.Status,
            LastValidatedAt = dto.LastValidatedAt
        };
}

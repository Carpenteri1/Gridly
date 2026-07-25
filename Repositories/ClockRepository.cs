using System.Data;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Services;

namespace Gridly.Repositories;

public class ClockRepository(IDbConnection connection, IDataConverter<ClockModel> dataConverter) : IClockRepository
{
    private DbCommandRunner _dbCommandRunner = new(connection);

    public async Task<(ClockModel? Clock, DateTime? FetchedAt)> Get(string location)
    {
        var dto = await _dbCommandRunner.Select<ClockDataDtoModel>(
            QueryStrings.SelectClockDataQuery, new { Location = location });

        if (dto is null) return (null, null);

        return (dataConverter.DeserializeJson(dto.JsonPayload), dto.FetchedAt);
    }

    public async Task<bool> Upsert(string location, ClockModel clock)
    {
        object parameters = new
        {
            Location = location,
            JsonPayload = dataConverter.SerializerToJsonString(clock),
            FetchedAt = DateTime.UtcNow
        };
        return await _dbCommandRunner.Execute(QueryStrings.UpsertClockDataQuery, parameters);
    }
}

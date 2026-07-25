using Gridly.EndPoints;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class ClockHandler(
    IClockEndPoint clockEndpoint,
    IClockRepository clockRepository) :
    IRequestHandler<GetClockQuery, IResult>,
    IRequestHandler<GetClockDataQuery, IResult>
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromMinutes(45);

    public async Task<IResult> Handle(GetClockQuery query, CancellationToken cancellationToken)
    {
        var (clock, fetchedAt) = await clockRepository.Get(query.SearchTerm);
        var isFresh = clock is not null && fetchedAt is not null && DateTime.UtcNow - fetchedAt.Value < CacheWindow;

        return isFresh ? Results.Ok(clock) : Results.NotFound();
    }

    public async Task<IResult> Handle(GetClockDataQuery query, CancellationToken cancellationToken)
    {
        var (status, clock) = await clockEndpoint.Get(query.SearchTerm);

        switch (status)
        {
            case ClockFetchStatus.Success:
                await clockRepository.Upsert(query.SearchTerm, clock!);
                return Results.Ok(clock);

            case ClockFetchStatus.ProviderUnavailable:
            default:
                var (staleClock, _) = await clockRepository.Get(query.SearchTerm);
                return staleClock is not null
                    ? Results.Ok(staleClock)
                    : Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: "ProviderUnavailable");
        }
    }
}

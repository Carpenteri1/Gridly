using Gridly.Models;
using Gridly.Querys;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gridly.Services;

public class VersionCheckBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private static readonly TimeSpan Interval = new VersionRateLimiterModel().Window;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckVersionAsync(stoppingToken);
            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    private async Task CheckVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new GetLatestVersionQuery(), cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

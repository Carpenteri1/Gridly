namespace Gridly.Services;

public abstract class PeriodicRefreshBackgroundService : BackgroundService
{
    protected abstract TimeSpan Interval { get; }
    protected readonly TimeSpan DelayBetweenProviderCalls = TimeSpan.FromSeconds(10);
    protected Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await RefreshData(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
    protected abstract Task RefreshData(CancellationToken stoppingToken);
}

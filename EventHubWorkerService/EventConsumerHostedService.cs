using Serilog;

namespace EventHubWorkerService.Services;

public class EventConsumerHostedService(EventConsumerService consumerService, ILogger<EventConsumerHostedService> logger) : BackgroundService
{
    private readonly EventConsumerService _consumerService = consumerService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Starting event consumer");
        await _consumerService.StartProcessingAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Stopping event consumer");
        await _consumerService.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
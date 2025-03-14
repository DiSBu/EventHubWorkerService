using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using EventHubWorkerService.Interfaces;
using EventHubWorkerService.Settings;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Serilog;
using System.Text.Json;

namespace EventHubWorkerService.Services;

public class EventHubProducerService : IEventHubProducerService, IDisposable
{
    private readonly EventHubProducerClient _producerClient;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;
    private readonly ResiliencePipeline _resiliencePipeline;

    public EventHubProducerService(
        IOptions<EventHubSettings> settings,
        ResiliencePipelineProvider<string> pipelineProvider,
        IEventHubClientFactory eventHubClientFactory,
        ILogger<EventHubProducerService> logger)
    {
        var credential = new DefaultAzureCredential();
        _producerClient = eventHubClientFactory.CreateEventHubProducerClient();
        _pipelineProvider = pipelineProvider;
        _resiliencePipeline = _pipelineProvider.GetPipeline("EventHubPipeline");
    }

    public async Task SendEventAsync<T>(T eventData, CancellationToken cancellationToken) where T : class
    {
        await _resiliencePipeline.ExecuteAsync(
            async token =>
            {
                using var eventBatch = await _producerClient.CreateBatchAsync(cancellationToken);

                var jsonData = JsonSerializer.Serialize(eventData);

                if (!eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(jsonData)))
                {
                    throw new InvalidOperationException("Failed to add event to batch");
                }

                Log.Information("Sending {EventType} as JSON: {JsonData}", typeof(T).Name, jsonData);
                await _producerClient.SendAsync(eventBatch, cancellationToken);
            },
        cancellationToken);
    }

    public void Dispose()
    {
        _producerClient.CloseAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}
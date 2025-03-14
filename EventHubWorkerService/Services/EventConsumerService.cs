using Azure.Messaging.EventHubs.Consumer;
using EventHubWorkerService.Interfaces;
using EventHubWorkerService.Models;
using EventHubWorkerService.Settings;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace EventHubWorkerService.Services;

public class EventConsumerService : IAsyncDisposable
{
    private readonly EventHubConsumerClient _consumer;
    private readonly JsonSerializerOptions _jsonOptions;

    public EventConsumerService(IOptions<EventHubSettings> settings, IEventHubClientFactory eventHubClientFactory)
    {
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _consumer = eventHubClientFactory.CreateEventHubConsumerClient();
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        Log.Information("Starting event processing");

        await foreach (var partitionEvent in _consumer.ReadEventsAsync(cancellationToken))
        {
            await ProcessEvent(partitionEvent);
        }
    }

    private async Task ProcessEvent(PartitionEvent partitionEvent)
    {
        try
        {
            var eventData = partitionEvent.Data;
            var json = eventData.EventBody.ToString();

            var eventModel = JsonSerializer.Deserialize<EventModel<TestEventModel>>(json, _jsonOptions);

            Log.Information(
                "Received CustomEvent - Device: {Device} | Value: {Value} | Partition: {Partition}",
                eventModel.Payload.Id,
                eventModel.Payload.Text,
                partitionEvent.Partition.PartitionId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing event");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _consumer.CloseAsync();
        await _consumer.DisposeAsync();
    }
}
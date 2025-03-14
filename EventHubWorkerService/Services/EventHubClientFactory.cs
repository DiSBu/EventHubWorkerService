using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using EventHubWorkerService.Interfaces;
using EventHubWorkerService.Settings;
using Microsoft.Extensions.Options;

namespace EventHubWorkerService.Services;

public sealed class EventHubClientFactory(IOptions<EventHubSettings> settings) : IEventHubClientFactory
{
    private readonly EventHubSettings _settings = settings.Value;

    public EventHubProducerClient CreateEventHubProducerClient()
    {
        return new EventHubProducerClient(
            _settings.Namespace,
            _settings.EventHubName,
            new DefaultAzureCredential());
    }
}
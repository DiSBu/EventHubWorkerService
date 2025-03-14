using Azure.Messaging.EventHubs.Producer;

namespace EventHubWorkerService.Interfaces
{
    public interface IEventHubClientFactory
    {
        EventHubProducerClient CreateEventHubProducerClient();
    }
}
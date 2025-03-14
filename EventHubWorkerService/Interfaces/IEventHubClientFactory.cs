using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubWorkerService.Interfaces
{
    public interface IEventHubClientFactory
    {
        EventHubConsumerClient CreateEventHubConsumerClient();
        EventHubProducerClient CreateEventHubProducerClient();
    }
}
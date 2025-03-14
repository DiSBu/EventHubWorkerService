using EventHubWorkerService.Models;

namespace EventHubWorkerService.Interfaces
{
    public interface IEventHubProducerService
    {
        Task SendEventAsync<T>(T eventData, CancellationToken cancellationToken) where T : class;
    }
}
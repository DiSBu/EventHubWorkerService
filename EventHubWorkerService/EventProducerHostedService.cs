using EventHubWorkerService.Interfaces;
using EventHubWorkerService.Models;
using Serilog;

namespace EventHubWorkerService;

public class EventProducerHostedService(IEventHubProducerService producerService) : BackgroundService
{
    private readonly IEventHubProducerService _producerService = producerService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO: ONLY FOR TESTING REMOVE THIS!
        TestEventModel payload = new TestEventModel() { Id = 1, Text = $"Test Event {DateTime.Now}" };
        EventModel<TestEventModel> eventData = new EventModel<TestEventModel>(eventType: "SampleEvent", payload: payload);

        await _producerService.SendEventAsync(eventData, stoppingToken);
        //TODO: ONLY FOR TESTING REMOVE THIS!

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Log.Debug("EventHubWorkerService is Active {Now}", DateTime.Now);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending event");
            }
        }
    }
}
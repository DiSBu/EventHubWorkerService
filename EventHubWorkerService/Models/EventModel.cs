namespace EventHubWorkerService.Models;

public class EventModel<T>
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public T Payload { get; set; }
    public string EventType { get; set; }

    public EventModel(T payload, string eventType)
    {
        Payload = payload;
        EventType = eventType;
    }
}
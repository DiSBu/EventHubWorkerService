namespace EventHubWorkerService.Settings;

public class EventHubSettings
{
    public string Namespace { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
}
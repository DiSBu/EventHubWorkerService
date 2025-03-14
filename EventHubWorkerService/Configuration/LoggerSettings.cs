namespace EventHubWorkerService.Configuration;

public class LoggerSettings
{
    public const string SectionName = "Serilog";

    public string[] Using { get; set; } = Array.Empty<string>();
    public MinimumLevelSettings MinimumLevel { get; set; } = new();
    public List<WriteToSettings> WriteTo { get; set; } = new();

    public class MinimumLevelSettings
    {
        public string Default { get; set; } = "Information";
        public Dictionary<string, string> Override { get; set; } = new();
    }

    public class WriteToSettings
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Args { get; set; } = new();
    }
}
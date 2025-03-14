namespace EventHubWorkerService.Settings;

public class RetryPolicySettings
{
    public int RetryCount { get; set; }
    public TimeSpan RetryDelay => TimeSpan.Parse(_retryDelay);
    public TimeSpan TimeoutDuration => TimeSpan.Parse(_timeoutDuration);
    public double CircuitBreakerThreshold { get; set; }
    public TimeSpan CircuitBreakerDuration => TimeSpan.Parse(_circuitBreakerDuration);

    private string _retryDelay = "00:00:02";
    private string _timeoutDuration = "00:00:10";
    private string _circuitBreakerDuration = "00:01:00";
}
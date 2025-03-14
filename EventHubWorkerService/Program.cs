using EventHubWorkerService.Settings;
using EventHubWorkerService.Utilities;
using Polly;
using Serilog;

namespace EventHubWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build())
            .CreateLogger();

        try
        {
            Log.Information("Starting worker service");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<EventHubSettings>(
                    hostContext.Configuration.GetSection(nameof(EventHubSettings)));

                services.Configure<RetryPolicySettings>(
                    hostContext.Configuration.GetSection(nameof(RetryPolicySettings)));

                const string key = "EventHubPipeline";

                services.AddResiliencePipeline(key, builder =>
                {
                    var settings = hostContext.Configuration.GetSection("RetryPolicySettings")
                        .Get<RetryPolicySettings>()!;

                    builder.AddRetry(new()
                    {
                        MaxRetryAttempts = settings.RetryCount,
                        Delay = settings.RetryDelay,
                        ShouldHandle = args => ValueTask.FromResult(args.Outcome.Exception is not null),
                        OnRetry = args =>
                        {
                            Log.Warning("Retry attempt {Attempt}", args.AttemptNumber);
                            return default;
                        }
                    });

                    builder.AddTimeout(settings.TimeoutDuration);

                    builder.AddCircuitBreaker(new()
                    {
                        FailureRatio = settings.CircuitBreakerThreshold,
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        MinimumThroughput = 2,
                        BreakDuration = settings.CircuitBreakerDuration,
                        ShouldHandle = args => ValueTask.FromResult(args.Outcome.Exception is not null)
                    });
                });

                services.AddEventHubWorkerServices(hostContext.Configuration);
            });
}
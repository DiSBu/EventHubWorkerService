﻿using EventHubWorkerService.Interfaces;
using EventHubWorkerService.Services;
using System.Diagnostics.CodeAnalysis;

namespace EventHubWorkerService.Utilities
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventHubWorkerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventHubClientFactory, EventHubClientFactory>();
            services.AddSingleton<IEventHubProducerService, EventHubProducerService>();
            services.AddHostedService<Worker>();

            return services;
        }
    }
}

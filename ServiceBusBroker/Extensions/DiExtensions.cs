using MessageBrokerCore.Extensions;
using MessageBrokerCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBusBroker.Extensions
{
    public static class DiExtensions
    {
        public static MessageBrokerConfigurator UseServiceBus(this MessageBrokerConfigurator cfg, Action<ServiceBusConfiguration, ServiceProvider> action)
        {
            cfg.Services.AddSingleton<ServiceBusConfiguration>();
            cfg.Services.AddSingleton<ServiceBusConnection>();
            cfg.Services.AddSingleton<ServiceBusAdministrationService>();
            cfg.Services.AddSingleton<ISubscriber, ServiceBusSubscriber>();
            cfg.Services.AddSingleton<IPublisher, ServiceBusPublisher>();
            var configuration = new ServiceBusConfiguration();
            action(configuration, cfg.Services.BuildServiceProvider());
            cfg.Services.AddSingleton(configuration);
            return cfg;
        }
    }
}

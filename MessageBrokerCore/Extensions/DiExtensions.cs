using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Processors;
using MessageBrokerCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBrokerCore.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, Action<MessageBrokerConfigurator> action)
        {
            services.AddSingleton<IConsumerContainer, ConsumerContainer>();
            services.AddSingleton<ISerializeService, SerializeService>();
            services.AddSingleton<IQueueNameService, QueueNameService>();
            var configurator = new MessageBrokerConfigurator(services);
            action(configurator);
            services.AddHostedService<DefaultHostedService>();
            return services;
        }
    }
}

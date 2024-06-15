using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBrokerCore.Extensions;

public class MessageBrokerConfigurator
{

    public MessageBrokerConfigurator(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; set; }
    
    public void AddConsumer<TConsumer, TConsumerImplementation, TIn>()
        where TConsumer : class
        where TConsumerImplementation : class, TConsumer
    {
        Services.AddSingleton<IConsumerWrapper, ConsumerWrapper<TIn>>();
        Services.AddScoped<TConsumer, TConsumerImplementation>();
    }

    public void AddConsumer<TConsumerImplementation, TIn>()
        where TConsumerImplementation : class, IConsumer<TIn>
    {
        Services.AddSingleton<IConsumerWrapper, ConsumerWrapper<TIn>>();
        Services.AddScoped<IConsumer<TIn>, TConsumerImplementation>();
    }
}
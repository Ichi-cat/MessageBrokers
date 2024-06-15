using Azure.Core;
using Azure.Messaging.ServiceBus;

namespace ServiceBusBroker;

public class ServiceBusConfiguration
{
    public ServiceBusConfiguration()
    {
        ClientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
    }

    public string? ConnectionString { get; set; }
    
    public ServiceBusClientOptions ClientOptions { get; init; }
}
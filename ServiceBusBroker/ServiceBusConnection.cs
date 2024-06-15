using Azure.Messaging.ServiceBus;

namespace ServiceBusBroker;

public class ServiceBusConnection : IAsyncDisposable
{
    private readonly ServiceBusClient _client;

    public ServiceBusConnection(ServiceBusConfiguration configuration)
    {
        _client = new ServiceBusClient(configuration.ConnectionString, configuration.ClientOptions);
    }

    public ServiceBusClient GetClient()
    {
        return _client;
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
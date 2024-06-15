using MessageBrokerCore.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MessageBrokerCore.Services
{
    public class DefaultHostedService : IHostedService
    {
        private readonly IConsumerContainer _container;

        public DefaultHostedService(IConsumerContainer container)
        {
            this._container = container;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _container.Start();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _container.DisposeAsync();
        }
    }
}

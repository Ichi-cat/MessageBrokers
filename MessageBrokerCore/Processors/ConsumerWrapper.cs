using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBrokerCore.Processors
{
    public class ConsumerWrapper<TIn> : IConsumerWrapper
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPublisher _publisher;

        public ConsumerWrapper(IServiceScopeFactory scopeFactory, IPublisher publisher)
        {
            _scopeFactory = scopeFactory;
            _publisher = publisher;
        }

        public async Task Consume(dynamic message)
        {
            var consumerModel = new ConsumerModel<TIn>(message, _publisher);
            using var scope = _scopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IConsumer<TIn>>().Consume(consumerModel);
        }
    }
}

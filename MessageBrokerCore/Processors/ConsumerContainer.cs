using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Models;
using System.Text;
using System.Text.Json;

namespace MessageBrokerCore.Processors
{
    public class ConsumerContainer : IConsumerContainer
    {
        private readonly IEnumerable<IConsumerWrapper> _consumers;

        private readonly ISubscriber _subscriber;

        public ConsumerContainer(IEnumerable<IConsumerWrapper> consumers, ISubscriber subscriber)
        {
            _consumers = consumers;
            _subscriber = subscriber;
        }

        public void Start()
        {
            AddConsumers();
        }

        private void AddConsumers()
        {
            foreach (var consumer in _consumers)
            {
                var methodConsume = consumer.GetType().GetMethod("Consume");
                var parameterType = methodConsume!.GetParameters().FirstOrDefault()?.ParameterType;
                if (parameterType == null)
                {
                    throw new Exception("Method must contain at least one parameter");
                }

                var modelType = consumer.GetType().GetGenericArguments()[0];
                var queueName = modelType.FullName!;
                _subscriber.AddReceiver(queueName, async (message) =>
                {
                    await consumer.Consume(message);
                }, modelType);
            }
        }



        public ValueTask DisposeAsync()
        {
            return _subscriber.DisposeAsync();
        }
    }
}

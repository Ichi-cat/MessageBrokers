using MessageBrokerCore.Models;

namespace MessageBrokerCore.Interfaces;

public interface IConsumer<TIn>
{
    Task Consume(ConsumerModel<TIn> model);
}
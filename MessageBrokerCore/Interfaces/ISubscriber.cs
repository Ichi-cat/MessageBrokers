using MessageBrokerCore.Models;

namespace MessageBrokerCore.Interfaces;

public interface ISubscriber : IAsyncDisposable
{
    public void AddReceiver<TIn>(string queueName, Func<MessageModel<TIn>, Task> func);
    public void AddReceiver(string queueName, Func<dynamic, Task> func, Type parameterType);
}
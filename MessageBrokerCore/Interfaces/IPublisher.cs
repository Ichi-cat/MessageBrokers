using MessageBrokerCore.Models;

namespace MessageBrokerCore.Interfaces;

public interface IPublisher
{
    Task SendAsync<TIn>(TIn message);
    Task<TOut?> SendAsync<TIn, TOut>(TIn message, CancellationToken cancellationToken = default);
    protected internal Task ReplyTo<TIn, TOut>(MessageModel<TIn> model, TOut newMessage);
}
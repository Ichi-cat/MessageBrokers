using MessageBrokerCore.Interfaces;

namespace MessageBrokerCore.Models;

public class ConsumerModel<TIn> : MessageModel<TIn>
{
    public ConsumerModel(TIn message, IPublisher context) : base(message)
    {
        Context = context;
    }

    public ConsumerModel(MessageModel<TIn> message, IPublisher context) : base(message)
    {
        Context = context;
    }

    public IPublisher Context { get; set; }

    public Task ReplyTo<TOut>(TOut message)
    {
        return Context.ReplyTo(this, message);
    }
}
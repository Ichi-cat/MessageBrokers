namespace MessageBrokerCore.Models;

public class MessageModel<TIn>
{
    public TIn Message { get; init; }
    public Metadata Metadata { get; init; } = new Metadata();

    public MessageModel(TIn message)
    {
        Message = message;
    }

    public MessageModel(TIn message, string? correlationId = null, string? replyQueue = null)
    {
        Message = message;
        Metadata = new Metadata
        {
            CorrelationId = correlationId,
            ReplyQueue = replyQueue
        };
    }

    public MessageModel(MessageModel<TIn> message)
    {
        Message = message.Message;
        Metadata = message.Metadata;
    }
}
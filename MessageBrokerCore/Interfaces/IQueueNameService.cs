namespace MessageBrokerCore.Interfaces
{
    public interface IQueueNameService
    {
        string GenerateQueueName<TIn>();
        string GenerateReplyQueueName<TIn>();
    }
}

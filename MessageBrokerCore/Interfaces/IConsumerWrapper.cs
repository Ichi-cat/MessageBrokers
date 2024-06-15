namespace MessageBrokerCore.Interfaces
{
    public interface IConsumerWrapper
    {
        Task Consume(dynamic message);
    }
}

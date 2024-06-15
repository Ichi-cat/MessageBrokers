namespace MessageBrokerCore.Interfaces
{
    public interface IConsumerContainer : IAsyncDisposable
    {
        public void Start();
    }
}

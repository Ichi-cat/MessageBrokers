using MessageBrokerCore.Interfaces;

namespace MessageBrokerCore.Services
{
    public class QueueNameService : IQueueNameService
    {
        public string GenerateQueueName<TIn>()
        {
            return typeof(TIn).FullName!;
        }

        public string GenerateReplyQueueName<TIn>()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

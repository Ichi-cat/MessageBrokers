using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MessageBrokerCore.Interfaces;

namespace ServiceBusBroker
{
    public class ServiceBusAdministrationService
    {
        private readonly ServiceBusAdministrationClient administrationClient;
        private readonly IQueueNameService queueNameService;

        public ServiceBusAdministrationService(ServiceBusConfiguration configuration, IQueueNameService queueNameService)
        {
            administrationClient = new ServiceBusAdministrationClient(configuration.ConnectionString);
            this.queueNameService = queueNameService;
        }

        public async Task CreateQueueAsync(string queueName)
        {
            try
            {
                await administrationClient.CreateQueueAsync(queueName);
            }
            catch (ServiceBusException ex)
            {
                if (!(ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists))
                {
                    throw;
                }
            }
        }

        public async Task<string> CreateTempQueue<TIn>()
        {
            string queueName = queueNameService.GenerateReplyQueueName<TIn>();
            var options = new CreateQueueOptions(queueName);

            QueueProperties createdQueue = await administrationClient.CreateQueueAsync(options);
            return createdQueue.Name;
        }
    }
}

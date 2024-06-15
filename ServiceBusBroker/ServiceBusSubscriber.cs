using Azure.Messaging.ServiceBus;
using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Models;

namespace ServiceBusBroker;

public class ServiceBusSubscriber : ISubscriber
{
    private bool isDisposing = false;

    private readonly ServiceBusClient client;
    private readonly ISerializeService serializeService;
    private readonly List<ServiceBusProcessor> processors = new List<ServiceBusProcessor>();

    public ServiceBusSubscriber(ServiceBusConnection connection, ISerializeService serializeService)
    {
        client = connection.GetClient();
        this.serializeService = serializeService;
    }

    public async void AddReceiver<TIn>(string queueName, Func<MessageModel<TIn>, Task> func)
    {
        await AddReceiver(queueName, async (request, metadata) =>
        {
            var deserializedObject = serializeService.DeserializeObject<TIn>(request);
            var messageModel = new MessageModel<TIn>(deserializedObject, metadata.CorrelationId, metadata.ReplyQueue);
            await func(messageModel);
        });
    }

    public async void AddReceiver(string queueName, Func<dynamic, Task> func, Type parameterType)
    {
        await AddReceiver(queueName, async (request, metadata) =>
        {
            dynamic deserializedObject = serializeService.DeserializeObject(request, parameterType);
            var modelType = typeof(MessageModel<>);
            var genericType = modelType.MakeGenericType(parameterType);
            var messageInstance = Activator.CreateInstance(genericType, deserializedObject, metadata.CorrelationId, metadata.ReplyQueue);
            await func(messageInstance);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (!isDisposing)
        {
            isDisposing = true;
            foreach (var processor in processors)
            {
                await processor.DisposeAsync();
            }
        }
    }

    private async Task AddReceiver(string queueName, Func<string, Metadata, Task> func)
    {
        var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true
        });
        processor.ProcessMessageAsync += async (args) =>
        {
            await func(args.Message.Body.ToString(), new Metadata
            {
                CorrelationId = args.Message.CorrelationId,
                ReplyQueue = args.Message.ReplyTo
            });
        };
        processor.ProcessErrorAsync += (args) =>
        {
            //todo: add message processor
            Console.WriteLine(args.ToString());
            return Task.CompletedTask;
        };
        await processor.StartProcessingAsync();
        processors.Add(processor);
    }
}
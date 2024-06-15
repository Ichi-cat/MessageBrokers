using System.Collections.Concurrent;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Models;

namespace ServiceBusBroker;

public class ServiceBusPublisher : IPublisher
{
    private readonly ServiceBusClient _client;
    private readonly ConcurrentBag<ServiceBusProcessor> serviceBusProcessors = new();
    private readonly ConcurrentDictionary<string, string> _callbackQueues = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
    private readonly ServiceBusAdministrationService administrataionService;
    private readonly ISerializeService serializeService;
    private readonly IQueueNameService queueNameService;

    public ServiceBusPublisher(ServiceBusConnection connection, 
        ServiceBusAdministrationService administrataionService, ISerializeService serializeService,
        IQueueNameService queueNameService)
    {
        _client = connection.GetClient();
        this.administrataionService = administrataionService;
        this.serializeService = serializeService;
        this.queueNameService = queueNameService;
    }
    
    public Task SendAsync<TIn>(TIn message)
    {
        var queueName = queueNameService.GenerateQueueName<TIn>();
        var model = new MessageModel<TIn>(message);
        return this.SendAsync(model, queueName);
    }

    public async Task SendAsync<TIn>(MessageModel<TIn> model, string queueName, CancellationToken token = default, bool createQueueIfNotExist = true)
    {
        var body = serializeService.ObjectToBytes(model.Message);
        var serviceBusMessage = new ServiceBusMessage(body)
        {
            CorrelationId = model.Metadata.CorrelationId,
            ReplyTo = model.Metadata.ReplyQueue
        };
        var sender = _client.CreateSender(queueName);
        try
        {
            await sender.SendMessageAsync(serviceBusMessage, token);
        } catch (ServiceBusException ex)
        {
            if(ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound && createQueueIfNotExist)
            {
                await administrataionService.CreateQueueAsync(queueName);
                await SendAsync(model, queueName, token, false);
            }
        }
    }

    public async Task<TOut?> SendAsync<TIn, TOut>(TIn message, CancellationToken cancellationToken = default)
    {
        var path = queueNameService.GenerateQueueName<TOut>();
        if (!_callbackQueues.TryGetValue(path, out var callbackQueueName))
        {
            callbackQueueName = await administrataionService.CreateTempQueue<TOut>();
            var callbackQueue = _client.CreateProcessor(callbackQueueName);
            
            serviceBusProcessors.Add(callbackQueue);
            _callbackQueues.TryAdd(path, callbackQueueName);
            callbackQueue.ProcessMessageAsync += async (args) =>
            {
                if (_callbackMapper.TryRemove(args.Message.CorrelationId, out var tcs))
                {
                    tcs.TrySetResult(args.Message.Body.ToString());
                }
                await args.CompleteMessageAsync(args.Message);
            };
            callbackQueue.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;//todo: add errors processing
            };

            await callbackQueue.StartProcessingAsync(cancellationToken);
        }
        var correlationId = Guid.NewGuid().ToString();
        
        var tcs = new TaskCompletionSource<string>();
        _callbackMapper.TryAdd(correlationId, tcs);

        var messageModel = new MessageModel<TIn>(message, correlationId, callbackQueueName);
        await this.SendAsync(messageModel, path, cancellationToken);
        
        cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out _));
        var serializedObject = await tcs.Task;
        if (serializedObject == null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<TOut>(serializedObject);
    }

    public Task ReplyTo<TIn, TOut>(MessageModel<TIn> model, TOut newMessage)
    {
        var messageModel = new MessageModel<TOut>(newMessage, model.Metadata.CorrelationId);
        return this.SendAsync(messageModel, model.Metadata.ReplyQueue);
    }
}
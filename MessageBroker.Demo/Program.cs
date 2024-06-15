using MessageBroker.Demo.Consumers;
using MessageBroker.Demo.Models;
using MessageBroker.Demo.Options;
using MessageBrokerCore.Extensions;
using Microsoft.Extensions.Options;
using ServiceBusBroker.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MessageBrokerOptions>(builder.Configuration.GetSection(OptionConstants.MessageBroker));
builder.Services.AddMessageBroker(cfg =>
{
    cfg.AddConsumer<PingConsumer, Ping>();
    cfg.UseServiceBus((sb, provider) =>
    {
        var options = provider.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;
        sb.ConnectionString = options.ConnectionString;
    });
});
var app = builder.Build();

app.Run();

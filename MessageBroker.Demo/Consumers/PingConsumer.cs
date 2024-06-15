using MessageBroker.Demo.Models;
using MessageBrokerCore.Interfaces;
using MessageBrokerCore.Models;

namespace MessageBroker.Demo.Consumers
{
    public class PingConsumer : IConsumer<Ping>
    {
        private readonly ILogger<PingConsumer> logger;

        public PingConsumer(ILogger<PingConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumerModel<Ping> model)
        {
            logger.LogInformation("Pressed {button}", model.Message.button);
            await model.ReplyTo(new Ping("w"));
        }
    }
}

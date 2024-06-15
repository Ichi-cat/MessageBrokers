using MessageBroker.Demo.Models;
using MessageBrokerCore.Interfaces;

namespace MessageBroker.Demo.BackgroundWorkers
{
    public class PingPublisher : BackgroundService
    {
        private readonly ILogger<PingPublisher> logger;
        private readonly IPublisher _publisher;

        public PingPublisher(ILogger<PingPublisher> logger, IPublisher publisher)
        {
            this.logger = logger;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                var keyPressed = Console.ReadKey(true);
                if (keyPressed.Key != ConsoleKey.Escape)
                {
                    var returnedButton = await _publisher.SendAsync<Ping, Ping>(new Ping(keyPressed.Key.ToString()));
                    logger.LogInformation("Returned button: {button}", returnedButton.button);
                }

                await Task.Delay(200);
            }
        }
    }
}

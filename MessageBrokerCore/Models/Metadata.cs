namespace MessageBrokerCore.Models
{
    public class Metadata
    {
        public string? CorrelationId { get; set; }

        public string? ReplyQueue { get; set; }

        public string? DeliveryTag { get; set; }
    }
}

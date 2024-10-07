using Microsoft.Extensions.Configuration;

namespace Order.Consumer.Configuration
{
    public class ConfigManager
    {
        private IConfiguration _configuration { get; }

        public ConfigManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OrderStatusQueueConfiguration OrderStatusQueueConfiguration => new OrderStatusQueueConfiguration
        {
            ExchangeName = _configuration.GetValue<string>("OrderStatusQueueConfiguration:ExchangeName"),
            ExchangeType = _configuration.GetValue<string>("OrderStatusQueueConfiguration:ExchangeType"),
            ExchangeDurable = _configuration.GetValue<bool>("OrderStatusQueueConfiguration:ExchangeDurable"),
            ExchangeAutoDelete = _configuration.GetValue<bool>("OrderStatusQueueConfiguration:ExchangeAutoDelete"),
            MessageTtl = _configuration.GetValue<int>("OrderStatusQueueConfiguration:MessageTtl"),
            QueueName = _configuration.GetValue<string>("OrderStatusQueueConfiguration:QueueName"),
            QueueDurable = _configuration.GetValue<bool>("OrderStatusQueueConfiguration:QueueDurable"),
            QueueExclusive = _configuration.GetValue<bool>("OrderStatusQueueConfiguration:QueueExclusive"),
            QueueAutoDelete = _configuration.GetValue<bool>("OrderStatusQueueConfiguration:QueueAutoDelete"),
            RoutingKey = _configuration.GetValue<string>("OrderStatusQueueConfiguration:RoutingKey"),
        };

        public NotificationEmailQueueConfiguration NotificationEmailQueueConfiguration => new NotificationEmailQueueConfiguration
        {
            ExchangeName = _configuration.GetValue<string>("NotificationEmailQueueConfiguration:ExchangeName"),
            ExchangeType = _configuration.GetValue<string>("NotificationEmailQueueConfiguration:ExchangeType"),
            ExchangeDurable = _configuration.GetValue<bool>("NotificationEmailQueueConfiguration:ExchangeDurable"),
            ExchangeAutoDelete = _configuration.GetValue<bool>("NotificationEmailQueueConfiguration:ExchangeAutoDelete"),
            MessageTtl = _configuration.GetValue<int>("NotificationEmailQueueConfiguration:MessageTtl"),
            QueueName = _configuration.GetValue<string>("NotificationEmailQueueConfiguration:QueueName"),
            QueueDurable = _configuration.GetValue<bool>("NotificationEmailQueueConfiguration:QueueDurable"),
            QueueExclusive = _configuration.GetValue<bool>("NotificationEmailQueueConfiguration:QueueExclusive"),
            QueueAutoDelete = _configuration.GetValue<bool>("NotificationEmailQueueConfiguration:QueueAutoDelete"),
            RoutingKey = _configuration.GetValue<string>("NotificationEmailQueueConfiguration:RoutingKey"),
        };

        public NotificationSmsQueueConfiguration NotificationSmsQueueConfiguration => new NotificationSmsQueueConfiguration
        {
            ExchangeName = _configuration.GetValue<string>("NotificationSmsQueueConfiguration:ExchangeName"),
            ExchangeType = _configuration.GetValue<string>("NotificationSmsQueueConfiguration:ExchangeType"),
            ExchangeDurable = _configuration.GetValue<bool>("NotificationSmsQueueConfiguration:ExchangeDurable"),
            ExchangeAutoDelete = _configuration.GetValue<bool>("NotificationSmsQueueConfiguration:ExchangeAutoDelete"),
            MessageTtl = _configuration.GetValue<int>("NotificationSmsQueueConfiguration:MessageTtl"),
            QueueName = _configuration.GetValue<string>("NotificationSmsQueueConfiguration:QueueName"),
            QueueDurable = _configuration.GetValue<bool>("NotificationSmsQueueConfiguration:QueueDurable"),
            QueueExclusive = _configuration.GetValue<bool>("NotificationSmsQueueConfiguration:QueueExclusive"),
            QueueAutoDelete = _configuration.GetValue<bool>("NotificationSmsQueueConfiguration:QueueAutoDelete"),
            RoutingKey = _configuration.GetValue<string>("NotificationSmsQueueConfiguration:RoutingKey"),
        };

    }

    public class OrderStatusQueueConfiguration
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool ExchangeDurable { get; set; }
        public bool ExchangeAutoDelete { get; set; }
        public int MessageTtl { get; set; }
        public string QueueName { get; set; }
        public bool QueueDurable { get; set; }
        public bool QueueExclusive { get; set; }
        public bool QueueAutoDelete { get; set; }
        public string RoutingKey { get; set; }
    }

    public class NotificationEmailQueueConfiguration
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool ExchangeDurable { get; set; }
        public bool ExchangeAutoDelete { get; set; }
        public int MessageTtl { get; set; }
        public string QueueName { get; set; }
        public bool QueueDurable { get; set; }
        public bool QueueExclusive { get; set; }
        public bool QueueAutoDelete { get; set; }
        public string RoutingKey { get; set; }
    }

    public class NotificationSmsQueueConfiguration
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool ExchangeDurable { get; set; }
        public bool ExchangeAutoDelete { get; set; }
        public int MessageTtl { get; set; }
        public string QueueName { get; set; }
        public bool QueueDurable { get; set; }
        public bool QueueExclusive { get; set; }
        public bool QueueAutoDelete { get; set; }
        public string RoutingKey { get; set; }
    }
}

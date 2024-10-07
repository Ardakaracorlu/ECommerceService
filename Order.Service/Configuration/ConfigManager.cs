using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Service.Configuration
{
    public class ConfigManager
    {
        private IConfiguration _configuration { get; }

        public ConfigManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StockQueueConfiguration StockQueueConfiguration => new StockQueueConfiguration
        {
            ExchangeName = _configuration.GetValue<string>("StockQueueConfiguration:ExchangeName"),
            ExchangeType = _configuration.GetValue<string>("StockQueueConfiguration:ExchangeType"),
            ExchangeDurable = _configuration.GetValue<bool>("StockQueueConfiguration:ExchangeDurable"),
            ExchangeAutoDelete = _configuration.GetValue<bool>("StockQueueConfiguration:ExchangeAutoDelete"),
            MessageTtl = _configuration.GetValue<int>("StockQueueConfiguration:MessageTtl"),
            QueueName = _configuration.GetValue<string>("StockQueueConfiguration:QueueName"),
            QueueDurable = _configuration.GetValue<bool>("StockQueueConfiguration:QueueDurable"),
            QueueExclusive = _configuration.GetValue<bool>("StockQueueConfiguration:QueueExclusive"),
            QueueAutoDelete = _configuration.GetValue<bool>("StockQueueConfiguration:QueueAutoDelete"),
            RoutingKey = _configuration.GetValue<string>("StockQueueConfiguration:RoutingKey"),
        };

    }

    public class StockQueueConfiguration
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

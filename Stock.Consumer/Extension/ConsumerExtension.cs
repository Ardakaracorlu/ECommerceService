using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.Consumer.Configuration;
using Stock.Consumer.Consumer;
using Stock.RabbitMQ.RabbitMQClient.Interface;

namespace Stock.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddHostedService<StockConsumer>();
            Stock.RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Stock.Data.Extension.DataExtension.RegisterService(services, configuration);
            ConfigureQueueSystem(services, configuration);
            return services;
        }

        public static void ConfigureQueueSystem(IServiceCollection services, IConfigurationRoot configuration)
        {
            var serviceBuilder = services.BuildServiceProvider();
            var queueService = serviceBuilder.GetService<IQueueOperation>();

            Dictionary<string, object> exchangeArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", configuration.GetSection("StockQueueConfiguration").GetValue<int>("MessageTtl") }
            };

            bool queueResult = false;
            bool exchangeResult = false;

            exchangeResult = queueService.ConfigureExchange(
                configuration.GetSection("StockQueueConfiguration").GetValue<string>("ExchangeName"),
                configuration.GetSection("StockQueueConfiguration").GetValue<string>("ExchangeType"),
                configuration.GetSection("StockQueueConfiguration").GetValue<bool>("ExchangeDurable"),
                configuration.GetSection("StockQueueConfiguration").GetValue<bool>("ExchangeAutoDelete"),
                exchangeArgs
            );

            if (exchangeResult)
                queueResult = queueService.ConfigureQueue(
                    configuration.GetSection("StockQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("StockQueueConfiguration").GetValue<bool>("QueueDurable"),
                    configuration.GetSection("StockQueueConfiguration").GetValue<bool>("QueueExclusive"),
                    configuration.GetSection("StockQueueConfiguration").GetValue<bool>("QueueAutoDelete"),
                    null
                );

            if (queueResult)
            {
                bool bindResult = queueService.BindQueueToExchange(
                    configuration.GetSection("StockQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("StockQueueConfiguration").GetValue<string>("ExchangeName"),
                    configuration.GetSection("StockQueueConfiguration").GetValue<string>("RoutingKey"),
                    null
                );
            }
        }
    }
}

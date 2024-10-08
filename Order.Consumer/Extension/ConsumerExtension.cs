using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Consumer.Configuration;
using Order.Consumer.Consumer;
using Order.Infrastructure.RabbitMQClient.Interface;

namespace Order.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddHostedService<OrderStatusConsumer>();
            Order.Infrastructure.Extension.InfrastructureExtension.RegisterService(services, configuration);
            ConfigureQueueSystem(services, configuration);
            return services;
        }

        public static void ConfigureQueueSystem(IServiceCollection services, IConfigurationRoot configuration)
        {
            var serviceBuilder = services.BuildServiceProvider();
            var queueService = serviceBuilder.GetService<IQueueOperation>();

            Dictionary<string, object> exchangeArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", configuration.GetSection("OrderStatusQueueConfiguration").GetValue<int>("MessageTtl") }
            };

            bool queueResult = false;
            bool exchangeResult = false;

            exchangeResult = queueService.ConfigureExchange(
                configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("ExchangeName"),
                configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("ExchangeType"),
                configuration.GetSection("OrderStatusQueueConfiguration").GetValue<bool>("ExchangeDurable"),
                configuration.GetSection("OrderStatusQueueConfiguration").GetValue<bool>("ExchangeAutoDelete"),
                exchangeArgs
            );

            if (exchangeResult)
                queueResult = queueService.ConfigureQueue(
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<bool>("QueueDurable"),
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<bool>("QueueExclusive"),
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<bool>("QueueAutoDelete"),
                    null
                );

            if (queueResult)
            {
                bool bindResult = queueService.BindQueueToExchange(
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("ExchangeName"),
                    configuration.GetSection("OrderStatusQueueConfiguration").GetValue<string>("RoutingKey"),
                    null
                );
            }
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Consumer.Configuration;
using Notification.Consumer.Consumer;
using Notification.Infrastructure.RabbitMQClient.Interface;

namespace Notification.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddHostedService<EmailConsumer>();
            services.AddHostedService<SmsConsumer>();
            Notification.Infrastructure.Extension.InfrastructureExtension.RegisterService(services, configuration);
            ConfigureNotificationEmailQueueSystem(services, configuration);
            ConfigureNotificationSmsQueueSystem(services, configuration);
            return services;
        }

        public static void ConfigureNotificationEmailQueueSystem(IServiceCollection services, IConfigurationRoot configuration)
        {
            var serviceBuilder = services.BuildServiceProvider();
            var queueService = serviceBuilder.GetService<IQueueOperation>();

            Dictionary<string, object> exchangeArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<int>("MessageTtl") }
            };

            bool queueResult = false;
            bool exchangeResult = false;

            exchangeResult = queueService.ConfigureExchange(
                configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("ExchangeName"),
                configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("ExchangeType"),
                configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<bool>("ExchangeDurable"),
                configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<bool>("ExchangeAutoDelete"),
                exchangeArgs
            );

            if (exchangeResult)
                queueResult = queueService.ConfigureQueue(
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<bool>("QueueDurable"),
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<bool>("QueueExclusive"),
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<bool>("QueueAutoDelete"),
                    null
                );

            if (queueResult)
            {
                bool bindResult = queueService.BindQueueToExchange(
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("ExchangeName"),
                    configuration.GetSection("NotificationEmailQueueConfiguration").GetValue<string>("RoutingKey"),
                    null
                );
            }

        }

        public static void ConfigureNotificationSmsQueueSystem(IServiceCollection services, IConfigurationRoot configuration)
        {
            var serviceBuilder = services.BuildServiceProvider();
            var queueService = serviceBuilder.GetService<IQueueOperation>();

            Dictionary<string, object> exchangeArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<int>("MessageTtl") }
            };

            bool queueResult = false;
            bool exchangeResult = false;

            exchangeResult = queueService.ConfigureExchange(
                configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("ExchangeName"),
                configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("ExchangeType"),
                configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<bool>("ExchangeDurable"),
                configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<bool>("ExchangeAutoDelete"),
                exchangeArgs
            );

            if (exchangeResult)
                queueResult = queueService.ConfigureQueue(
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<bool>("QueueDurable"),
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<bool>("QueueExclusive"),
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<bool>("QueueAutoDelete"),
                    null
                );

            if (queueResult)
            {
                bool bindResult = queueService.BindQueueToExchange(
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("QueueName"),
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("ExchangeName"),
                    configuration.GetSection("NotificationSmsQueueConfiguration").GetValue<string>("RoutingKey"),
                    null
                );
            }

        }
    }
}

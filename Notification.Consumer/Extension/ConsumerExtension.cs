using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Consumer.Configuration;
using Notification.Consumer.Consumer;

namespace Notification.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddHostedService<EmailConsumer>();
            services.AddHostedService<SmsConsumer>();
            Notification.RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Notification.Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

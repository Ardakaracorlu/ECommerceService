using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Consumer.Configuration;
using Order.Consumer.Consumer;

namespace Order.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddHostedService<OrderStatusConsumer>();
            Order.RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Order.Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

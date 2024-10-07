using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Consumer.Consumer;

namespace Order.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddHostedService<OrderStatusConsumer>();
            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Order.Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.Consumer.Consumer;

namespace Stock.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddHostedService<StockConsumer>();
            Stock.RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Stock.Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

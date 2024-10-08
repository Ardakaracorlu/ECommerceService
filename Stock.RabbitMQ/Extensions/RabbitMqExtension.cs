using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Stock.RabbitMQ.RabbitMQClient.Base;
using Stock.RabbitMQ.RabbitMQClient.Interface;

namespace Stock.RabbitMQ.Extensions
{
    public static class RabbitMqExtension
    {
        public static IServiceCollection RegisterRabbitMqExtension(this IServiceCollection services, IConfigurationRoot configuration)
        {
           

            return services;
        }
    }
}

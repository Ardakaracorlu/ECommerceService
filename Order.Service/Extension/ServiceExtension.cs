using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Service.Service;

namespace Order.Service.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddScoped<ICreateOrderService, CreateOrderService>();
            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Order.Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

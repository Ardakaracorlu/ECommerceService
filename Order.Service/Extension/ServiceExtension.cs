using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Service.Configuration;
using Order.Service.Service;

namespace Order.Service.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<ConfigManager>();
            services.AddScoped<ICreateOrderService, CreateOrderService>();
            Order.Infrastructure.Extension.InfrastructureExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

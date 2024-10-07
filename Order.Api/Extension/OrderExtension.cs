using Microsoft.EntityFrameworkCore;
using Order.Api.Consumer;
using Order.Api.Data.Context;

namespace Order.Api.Extension
{
    public static class OrderExtension
    {
        public static  IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddHostedService<OrderStatusConsumer>();
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:OrderDbConnection").Value);
            });

            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            return services;
        }
    }
}

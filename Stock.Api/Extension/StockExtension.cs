using Microsoft.EntityFrameworkCore;
using Stock.Api.Consumer;
using Stock.Api.Data.Context;

namespace Order.Api.Extension
{
    public static class StockExtension
    {
        public static  IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {

            services.AddHostedService<StockConsumer>();
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<StockDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:StockDbConnection").Value);
            });

            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            return services;
        }
    }
}

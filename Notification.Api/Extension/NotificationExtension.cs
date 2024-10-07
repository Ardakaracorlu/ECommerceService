using Microsoft.EntityFrameworkCore;
using Notification.Api.Consumer;
using Notification.Api.Data.Context;

namespace Notification.Api.Extension
{
    public static class NotificationExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {

            services.AddHostedService<EmailConsumer>();
            services.AddHostedService<SmsConsumer>();
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:NotificationDbConnection").Value);
            });

            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            return services;
        }
    }
}

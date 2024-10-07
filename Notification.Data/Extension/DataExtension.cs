using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Data.Context;

namespace Notification.Data.Extension
{
    public static class DataExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:NotificationDbConnection").Value);
            });

            return services;
        }
    }
}

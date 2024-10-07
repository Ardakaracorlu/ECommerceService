using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Data.Context;

namespace Order.Data.Extension
{
    public static class DataExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:OrderDbConnection").Value);
            });

            return services;
        }
    }
}

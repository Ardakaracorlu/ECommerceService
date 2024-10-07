using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.Data.Context;

namespace Stock.Data.Extension
{
    public static class DataExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<StockDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:StockDbConnection").Value);
            });

            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.Data.Context;
using Order.Infrastructure.RabbitMQClient.Base;
using Order.Infrastructure.RabbitMQClient.Interface;
using RabbitMQ.Client;

namespace Order.Infrastructure.Extension
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            #region Postgres
            //PostgreSql Timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetSection("ConnectionStrings:OrderDbConnection").Value);
            });
            #endregion

            #region RabbitMQ
            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMQConfiguration").GetValue<string>("Host"),
                Port = Convert.ToInt32(configuration.GetSection("RabbitMQConfiguration").GetValue<string>("Port")),
                UserName = configuration.GetSection("RabbitMQConfiguration").GetValue<string>("Username"),
                Password = configuration.GetSection("RabbitMQConfiguration").GetValue<string>("Password")
            };
            // Otomatik bağlantı kurtarmayı etkinleştirmek için,
            connectionFactory.AutomaticRecoveryEnabled = true;
            // Her 10 sn de bir tekrar bağlantı toparlanmaya çalışır 
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            // sunucudan bağlantısı kesildikten sonra kuyruktaki mesaj tüketimini sürdürmez 
            // (TopologyRecoveryEnabled = false olarak tanımlandığı için)
            connectionFactory.TopologyRecoveryEnabled = false;
            IConnection? rabbitConnection = connectionFactory.CreateConnection();
            services.AddSingleton(rabbitConnection);

            services.AddSingleton<IQueueOperation, QueueOperation>();
            #endregion


            return services;
        }
    }
}

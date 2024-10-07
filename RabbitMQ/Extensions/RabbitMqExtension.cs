using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.RabbitMQClient.Base;
using RabbitMQ.RabbitMQClient.Interface;

namespace RabbitMQ.Extensions
{
    public static class RabbitMqExtension
    {
        public static IServiceCollection RegisterRabbitMqExtension(this IServiceCollection services, IConfigurationRoot configuration)
        {
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

            return services;
        }
    }
}

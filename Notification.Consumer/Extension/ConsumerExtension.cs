﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Consumer.Consumer;

namespace Notification.Consumer.Extension
{
    public static class ConsumerExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddHostedService<EmailConsumer>();
            services.AddHostedService<SmsConsumer>();
            RabbitMQ.Extensions.RabbitMqExtension.RegisterRabbitMqExtension(services, configuration);
            Data.Extension.DataExtension.RegisterService(services, configuration);
            return services;
        }
    }
}

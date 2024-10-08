using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Consumer.Configuration;
using Notification.Consumer.Model.Response;
using Notification.Infrastructure.Data.Constants;
using Notification.Infrastructure.Data.Context;
using Notification.Infrastructure.Data.Entities;
using Notification.Infrastructure.Helper;
using Notification.Infrastructure.RabbitMQClient.Interface;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification.Consumer.Consumer
{
    public class SmsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;
        private readonly ConfigManager _configManager;

        public SmsConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation, ConfigManager configManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
            _configManager = configManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue(_configManager.NotificationSmsQueueConfiguration.QueueName,
                _configManager.NotificationSmsQueueConfiguration.ExchangeName,
                _configManager.NotificationSmsQueueConfiguration.ExchangeType,
                _configManager.NotificationSmsQueueConfiguration.RoutingKey, 1, receivedEventHandler: (model, ea) =>
                {

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _notificationDbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                        var retryPolicy = RetryPolicyHelper.GetRetryPolicy();

                        retryPolicy.Execute(() =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var smsResponse = JsonSerializer.Deserialize<NotificationSmsResponse>(message);

                            SendSms(smsResponse.Message, smsResponse.Phone);

                            NotificationInfo notificationInfo = new NotificationInfo
                            {
                                Recipient = smsResponse.Phone,
                                Message = smsResponse.Message,
                                NotificationStatus = NotificationStatus.Sent,
                                NotificationType = NotificationType.Sms,
                                OrderId = smsResponse.OrderId,
                            };

                            _notificationDbContext.Add(notificationInfo);
                            _notificationDbContext.SaveChanges();

                            ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                        });
                    }
                });

            return Task.CompletedTask;
        }

        private void SendSms(string message, string phone)
        {
            Console.WriteLine(message);
            Console.WriteLine(phone);
        }
    }
}

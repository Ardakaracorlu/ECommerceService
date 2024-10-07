using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Consumer.Model.Response;
using Notification.Data.Constants;
using Notification.Data.Context;
using Notification.Data.Entities;
using RabbitMQ.Client.Events;
using RabbitMQ.RabbitMQClient.Interface;
using System.Text;
using System.Text.Json;

namespace Notification.Consumer.Consumer
{
    public class SmsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;

        public SmsConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue("notification_sms", "notification_topic", "topic", "notification_sms_key", 1, receivedEventHandler: (model, ea) =>
                {

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _notificationDbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

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
                    }
                });

            // Sürekli dinlemeyi sağla
            return Task.CompletedTask;
        }

        private void SendSms(string message, string phone)
        {
            Console.WriteLine(message);
            Console.WriteLine(phone);
        }
    }
}

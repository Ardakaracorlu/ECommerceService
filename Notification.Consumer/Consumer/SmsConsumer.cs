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
        private readonly IServiceProvider _serviceProvider;

        public SmsConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _notificationDbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                    var _queueOperation = scope.ServiceProvider.GetRequiredService<IQueueOperation>();

                    _queueOperation.ConsumeQueue("notification_sms", "notification_topic", "topic", "notification_sms_key", 1, receivedEventHandler: (model, ea) =>
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

                // Bir sonraki dinleme döngüsüne geçmeden önce kısa bir bekleme süresi
                await Task.Delay(1000, stoppingToken); // Örneğin 1 saniye
            }
        }

        private void SendSms(string message, string phone)
        {
            Console.WriteLine(message);
            Console.WriteLine(phone);
        }
    }
}

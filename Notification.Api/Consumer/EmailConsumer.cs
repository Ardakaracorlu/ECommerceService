
using Notification.Api.Data.Constants;
using Notification.Api.Data.Context;
using Notification.Api.Data.Entities;
using Notification.Api.Model.Response;
using RabbitMQ.Client.Events;
using RabbitMQ.RabbitMQClient.Interface;
using System.Text;
using System.Text.Json;

namespace Notification.Api.Consumer
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailConsumer(IServiceProvider serviceProvider)
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

                    _queueOperation.ConsumeQueue("notification_email", "notification_topic", "topic", "notification_email_key", 1, receivedEventHandler: (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var emailResponse = JsonSerializer.Deserialize<NotificationEmailResponse>(message);

                        SendMail(emailResponse.Message, emailResponse.Email);

                        NotificationInfo notificationInfo = new NotificationInfo
                        {
                            Recipient = emailResponse.Email,
                            Message = emailResponse.Message,
                            NotificationStatus = NotificationStatus.Sent,
                            NotificationType = NotificationType.Email,
                            OrderId = emailResponse.OrderId,
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

        private void SendMail(string message, string eMail)
        {
            Console.WriteLine(message);
            Console.WriteLine(eMail);
        }
    }
}

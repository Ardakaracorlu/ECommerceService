using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Consumer.Configuration;
using Notification.Consumer.Model.Response;
using Notification.Data.Constants;
using Notification.Data.Context;
using Notification.Data.Entities;
using Notification.RabbitMQ.RabbitMQClient.Interface;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification.Consumer.Consumer
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;
        private readonly ConfigManager _configManager;

        public EmailConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation, ConfigManager configManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
            _configManager = configManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue(_configManager.NotificationEmailQueueConfiguration.QueueName,
                _configManager.NotificationEmailQueueConfiguration.ExchangeName,
                _configManager.NotificationEmailQueueConfiguration.ExchangeType,
                _configManager.NotificationEmailQueueConfiguration.RoutingKey,
                1, receivedEventHandler: (model, ea) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _notificationDbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();


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
                }
            });

            // Sürekli dinlemeyi sağla
            return Task.CompletedTask;
        }


        private void SendMail(string message, string eMail)
        {
            Console.WriteLine(message);
            Console.WriteLine(eMail);
        }
    }
}

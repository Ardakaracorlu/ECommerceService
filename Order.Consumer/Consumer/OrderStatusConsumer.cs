using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Consumer.Configuration;
using Order.Consumer.Model.Request;
using Order.Consumer.Model.Response;
using Order.Data.Constants;
using Order.Data.Context;
using Order.RabbitMQ.RabbitMQClient.Interface;
using RabbitMQ.Client.Events;
using Stock.Common.Helper;
using System.Text;
using System.Text.Json;

namespace Order.Consumer.Consumer
{
    public class OrderStatusConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;
        private readonly ConfigManager _configManager;

        public OrderStatusConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation, ConfigManager configManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
            _configManager = configManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue(_configManager.OrderStatusQueueConfiguration.QueueName,
                _configManager.OrderStatusQueueConfiguration.ExchangeName,
                _configManager.OrderStatusQueueConfiguration.ExchangeType,
                _configManager.OrderStatusQueueConfiguration.RoutingKey, 1, receivedEventHandler: (model, ea) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                    var retryPolicy = RetryPolicyHelper.GetRetryPolicy();

                    retryPolicy.Execute(() =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var orderStatusResponse = JsonSerializer.Deserialize<OrderStatusResponse>(message);

                        var orderData = _orderDbContext.OrdersInfo.SingleOrDefault(x => x.Id == orderStatusResponse.OrderId);
                        string customerMessage = string.Empty;


                        if (orderStatusResponse.Status)
                        {
                            orderData.Status = OrderStatus.OrderProcessing;
                            orderData.Description = orderStatusResponse.Message;
                            orderData.UpdatedAt = DateTime.Now;
                            customerMessage = $"Sayın {orderData.CustomerName} {orderData.CustomerSurname} Siparişiniz hazırlanıyor";
                        }
                        else
                        {
                            orderData.Status = OrderStatus.OrderCanceled;
                            orderData.Description = orderStatusResponse.Message;
                            orderData.UpdatedAt = DateTime.Now;
                            customerMessage = $"Sayın {orderData.CustomerName} {orderData.CustomerSurname} Siparişiniz iptal edildi";
                        }

                        _orderDbContext.Update(orderData);
                        _orderDbContext.SaveChanges();

                        _queueOperation.PublishMessage(new NotificationEmailRequest
                        {
                            OrderId = orderData.Id,
                            Email = orderData.Email,
                            Message = customerMessage,
                        }, _configManager.NotificationEmailQueueConfiguration.QueueName,
                        _configManager.NotificationEmailQueueConfiguration.ExchangeName,
                        _configManager.NotificationEmailQueueConfiguration.RoutingKey,
                        _configManager.NotificationEmailQueueConfiguration.MessageTtl);

                        _queueOperation.PublishMessage(new NotificationSmsRequest
                        {
                            OrderId = orderData.Id,
                            Phone = orderData.Phone,
                            Message = customerMessage
                        }, _configManager.NotificationSmsQueueConfiguration.QueueName,
                        _configManager.NotificationSmsQueueConfiguration.ExchangeName,
                        _configManager.NotificationSmsQueueConfiguration.RoutingKey,
                        _configManager.NotificationSmsQueueConfiguration.MessageTtl);

                        ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                    });
                }
            });
            return Task.CompletedTask;

        }
    }
}

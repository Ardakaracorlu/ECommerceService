﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Consumer.Model.Request;
using Order.Consumer.Model.Response;
using Order.Data.Constants;
using Order.Data.Context;
using RabbitMQ.Client.Events;
using RabbitMQ.RabbitMQClient.Interface;
using System.Text;
using System.Text.Json;

namespace Order.Consumer.Consumer
{
    public class OrderStatusConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;

        public OrderStatusConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue("order_status_queue", "order_status_direct", "direct", "order_status_key", 1, receivedEventHandler: (model, ea) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

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
                    }, "notification_email", "notification_topic", "notification_email_key", 0);

                    _queueOperation.PublishMessage(new NotificationSmsRequest
                    {
                        OrderId = orderData.Id,
                        Phone = orderData.Phone,
                        Message = customerMessage
                    }, "notification_sms", "notification_topic", "notification_sms_key", 0);

                    ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                }
            });

            // Sürekli dinlemeyi sağla
            return Task.CompletedTask;

        }
    }
}

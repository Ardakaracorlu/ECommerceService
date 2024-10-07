using Microsoft.EntityFrameworkCore;
using Order.Api.Data.Constants;
using Order.Api.Data.Context;
using Order.Api.Model.Request;
using Order.Api.Model.Response;
using RabbitMQ.Client.Events;
using RabbitMQ.RabbitMQClient.Interface;
using System.Text;
using System.Text.Json;

namespace Order.Api.Consumer
{
    public class OrderStatusConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderStatusConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                    var _queueOperation = scope.ServiceProvider.GetRequiredService<IQueueOperation>();

                    _queueOperation.ConsumeQueue("order_status_queue", "order_status_direct", "direct", "order_status_key", 1, receivedEventHandler: (model, ea) =>
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
                        }, "notification_email", "notification_topic", "notification_email_key", 0);

                        _queueOperation.PublishMessage(new NotificationSmsRequest
                        {
                            OrderId = orderData.Id,
                            Phone = orderData.Phone,
                            Message = customerMessage
                        }, "notification_sms", "notification_topic", "notification_sms_key", 0);

                        ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                    });

                }

                // Bir sonraki dinleme döngüsüne geçmeden önce kısa bir bekleme süresi
                await Task.Delay(1000, stoppingToken); // Örneğin 1 saniye
            }
        }
    }
}

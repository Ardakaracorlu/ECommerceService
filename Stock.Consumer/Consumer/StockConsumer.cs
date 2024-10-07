using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.RabbitMQClient.Interface;
using Stock.Consumer.Model.Request;
using Stock.Consumer.Model.Response;
using Stock.Data.Context;
using System.Text;
using System.Text.Json;

namespace Stock.Consumer.Consumer
{
    public class StockConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public StockConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _stockDbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                    var _queueOperation = scope.ServiceProvider.GetRequiredService<IQueueOperation>();

                    _queueOperation.ConsumeQueue("stock_queue", "stock.direct", "direct", "stock_key", 1, receivedEventHandler: (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var stockResponse = JsonSerializer.Deserialize<StockQueueResponse>(message);


                        var stockData = _stockDbContext.StocksInfo.SingleOrDefault(x => x.ProductId == stockResponse.ProductId);
                        OrderStatusRequest orderStatusRequest = new OrderStatusRequest
                        {
                            OrderId = stockResponse.OrderId
                        };

                        if (stockData == null)
                        {
                            orderStatusRequest.Message = "Ürün Bulunamadı";
                            _queueOperation.PublishMessage(orderStatusRequest, "order_status_queue", "order_status_direct", "order_status_key", 0);
                            ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);

                            return;
                        }

                        if (stockResponse.Quantity > stockData.Quantity)
                        {
                            orderStatusRequest.Message = "Ürün için Yeterli Stok Durumu bulunamadı";
                            _queueOperation.PublishMessage(orderStatusRequest, "order_status_queue", "order_status_direct", "order_status_key", 0);
                            ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);

                            return;
                        }
                        stockData.Quantity -= stockResponse.Quantity;
                        stockData.UpdatedAt = DateTime.Now;
                        _stockDbContext.Update(stockData);
                        _stockDbContext.SaveChanges();


                        orderStatusRequest.Message = "Sipariş Hazırlanıyor";
                        orderStatusRequest.Status = true;
                        _queueOperation.PublishMessage(orderStatusRequest, "order_status_queue", "order_status_direct", "order_status_key", 0);

                        ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);

                    });
                }
                // Bir sonraki dinleme döngüsüne geçmeden önce kısa bir bekleme süresi
                await Task.Delay(1000, stoppingToken); // Örneğin 1 saniye
            }

        }
    }
}

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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;

        public StockConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // RabbitMQ'dan mesajları dinlemeye başla
            _queueOperation.ConsumeQueue("stock_queue", "stock.direct", "direct", "stock_key", 1, async (model, ea) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _stockDbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var stockResponse = JsonSerializer.Deserialize<StockQueueResponse>(message);

                    // Mesajın işlenmesi ve veritabanı işlemleri
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

                    // Stok güncelleme
                    stockData.Quantity -= stockResponse.Quantity;
                    stockData.UpdatedAt = DateTime.Now;
                    _stockDbContext.Update(stockData);
                    await _stockDbContext.SaveChangesAsync();

                    // Başarılı işlem sonrası mesaj gönderimi
                    orderStatusRequest.Message = "Sipariş Hazırlanıyor";
                    orderStatusRequest.Status = true;
                    _queueOperation.PublishMessage(orderStatusRequest, "order_status_queue", "order_status_direct", "order_status_key", 0);

                    ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                }
            });

            // Sürekli dinlemeyi sağla
            return Task.CompletedTask;
        }
    }
}

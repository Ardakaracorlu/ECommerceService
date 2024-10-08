using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using Stock.Consumer.Configuration;
using Stock.Consumer.Model.Request;
using Stock.Consumer.Model.Response;
using Stock.Infrastructure.Data.Context;
using Stock.Infrastructure.Helper;
using Stock.Infrastructure.RabbitMQClient.Interface;
using System.Text;
using System.Text.Json;

namespace Stock.Consumer.Consumer
{
    public class StockConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueOperation _queueOperation;
        private readonly ConfigManager _configManager;

        public StockConsumer(IServiceScopeFactory serviceScopeFactory, IQueueOperation queueOperation, ConfigManager configManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queueOperation = queueOperation;
            _configManager = configManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueOperation.ConsumeQueue(_configManager.StockQueueConfiguration.QueueName,
                _configManager.StockQueueConfiguration.ExchangeName,
                _configManager.StockQueueConfiguration.ExchangeType,
                _configManager.StockQueueConfiguration.RoutingKey,
                1, async (model, ea) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _stockDbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                    var retryPolicy = RetryPolicyHelper.GetRetryPolicy();

                    retryPolicy.Execute(() =>
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
                            _queueOperation.PublishMessage(orderStatusRequest, _configManager.OrderStatusQueueConfiguration.QueueName,
                                _configManager.OrderStatusQueueConfiguration.ExchangeName,
                                _configManager.OrderStatusQueueConfiguration.RoutingKey,
                                _configManager.OrderStatusQueueConfiguration.MessageTtl);
                            ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                            return;
                        }

                        if (stockResponse.Quantity > stockData.Quantity)
                        {
                            orderStatusRequest.Message = "Ürün için Yeterli Stok Durumu bulunamadı";
                            _queueOperation.PublishMessage(orderStatusRequest, _configManager.OrderStatusQueueConfiguration.QueueName,
                                _configManager.OrderStatusQueueConfiguration.ExchangeName,
                                _configManager.OrderStatusQueueConfiguration.RoutingKey,
                                _configManager.OrderStatusQueueConfiguration.MessageTtl);
                            ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                            return;
                        }

                        stockData.Quantity -= stockResponse.Quantity;
                        stockData.UpdatedAt = DateTime.Now;
                        _stockDbContext.Update(stockData);
                        _stockDbContext.SaveChanges();

                        orderStatusRequest.Message = "Sipariş Hazırlanıyor";
                        orderStatusRequest.Status = true;
                        _queueOperation.PublishMessage(orderStatusRequest, _configManager.OrderStatusQueueConfiguration.QueueName,
                                _configManager.OrderStatusQueueConfiguration.ExchangeName,
                                _configManager.OrderStatusQueueConfiguration.RoutingKey,
                                _configManager.OrderStatusQueueConfiguration.MessageTtl);

                        ((EventingBasicConsumer)model).Model.BasicAck(ea.DeliveryTag, false);
                    }); 
                }
            });
            return Task.CompletedTask;
        }
    }
}

using Order.Common.Helper;
using Order.Data.Constants;
using Order.Data.Context;
using Order.Data.Entities;
using Order.RabbitMQ.RabbitMQClient.Interface;
using Order.Service.Configuration;
using Order.Service.Model.Request;

namespace Order.Service.Service
{
    public interface ICreateOrderService
    {
        Task<string> CreateOrder(CreateOrderRequest createOrderRequest);
    }
    public class CreateOrderService : ICreateOrderService
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly IQueueOperation _queueOperation;
        private readonly ConfigManager _configManager;

        public CreateOrderService(OrderDbContext orderDbContext, IQueueOperation queueOperation, ConfigManager configManager)
        {
            _orderDbContext = orderDbContext;
            _queueOperation = queueOperation;
            _configManager = configManager;
        }

        public async Task<string> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            OrderInfo orderInfo = new OrderInfo
            {
                ProductId = createOrderRequest.ProductId,
                Quantity = createOrderRequest.Quantity,
                CustomerName = createOrderRequest.CustomerName,
                CustomerSurname = createOrderRequest.CustomerSurname,
                Phone = createOrderRequest.Phone,
                Adress = createOrderRequest.Adress,
                Email = createOrderRequest.Email,
                Status = OrderStatus.OrderReceived,
                Description = "Sipariş Alındı"
            };

            await _orderDbContext.AddAsync(orderInfo);
            await _orderDbContext.SaveChangesAsync();

            StockQueueRequest stockQueueRequest = new StockQueueRequest
            {
                OrderId = orderInfo.Id,
                ProductId = createOrderRequest.ProductId,
                Quantity = createOrderRequest.Quantity,
            };

            var retryPolicy = RetryPolicyHelper.GetRetryPolicy();
            retryPolicy.Execute(() =>
            {
                _queueOperation.PublishMessage(stockQueueRequest,
                  _configManager.StockQueueConfiguration.QueueName,
                  _configManager.StockQueueConfiguration.ExchangeName,
                  _configManager.StockQueueConfiguration.RoutingKey,
                  _configManager.StockQueueConfiguration.MessageTtl);
            });


              

            return "Siparişiniz Başarıyla Alındı";
        }
    }
}

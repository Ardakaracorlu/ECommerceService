using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Api.Data.Constants;
using Order.Api.Data.Context;
using Order.Api.Data.Entities;
using Order.Api.Model.Request;
using RabbitMQ.RabbitMQClient.Interface;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderDbContext _orderDbContext;
        private readonly IQueueOperation _queueOperation;

        public OrderController(OrderDbContext orderDbContext, IQueueOperation queueOperation)
        {
            _orderDbContext = orderDbContext;
            _queueOperation = queueOperation;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            for (int i = 0; i < 5; i++)
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

                _queueOperation.PublishMessage(stockQueueRequest, "stock_queue", "stock.direct", "stock_key", 0); // StockQueue
            }
            return Ok("Siparişiniz Başarıyla Alındı");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Order.Service.Model.Request;
using Order.Service.Service;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly ICreateOrderService _createOrderService;

        public OrderController(ICreateOrderService createOrderService)
        {
           _createOrderService = createOrderService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            
            return Ok(await _createOrderService.CreateOrder(createOrderRequest));
        }
    }
}

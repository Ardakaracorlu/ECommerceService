using Microsoft.AspNetCore.Mvc;

namespace Stock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {

            return Ok();
        }
    }
}

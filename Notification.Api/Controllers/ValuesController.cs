using Microsoft.AspNetCore.Mvc;

namespace Notification.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}

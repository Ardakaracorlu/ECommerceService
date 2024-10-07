using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notification.Api.Data.Context;

namespace Notification.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly NotificationDbContext _context;

        public ValuesController(NotificationDbContext notificationDbContext)
        {
            _context = notificationDbContext;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var asd = await _context.NotificationInfo.ToListAsync();

            return Ok();
        }
    }
}

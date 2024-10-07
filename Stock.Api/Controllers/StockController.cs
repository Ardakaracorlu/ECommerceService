using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock.Api.Data.Context;

namespace Stock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockDbContext _stockDbContext;

        public StockController(StockDbContext stockDbContext)
        {
            _stockDbContext = stockDbContext;
        }

        [HttpGet("GetStocks")]
        public async Task<IActionResult> GetStocks()
        {
            var asd = await _stockDbContext.StocksInfo.ToListAsync();

            return Ok(asd);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using CollecthubDotNet.Models;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "pong",
                Data = new
                {
                    Status = "pong",
                    Timestamp = DateTime.UtcNow,
                    Server = Environment.MachineName,
                    Version = "1.0.0"
                }
            });
        }
    }
}
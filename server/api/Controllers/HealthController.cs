using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace api.Controllers
{
    [ApiController]
    [Route("api/Health")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("App/Up")]
        public IActionResult Up()
        {
            return Ok(new
            {
                code = 200,
                status = "up",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("Database/Up")]
        public async Task<IActionResult> DatabaseUp()
        {
            var connectionString = _configuration["AppOptions:Db"];

            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                return Ok(new
                {
                    code = 200,
                    status = "up",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    code = 503,
                    status = "down",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
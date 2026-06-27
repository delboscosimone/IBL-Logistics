using Microsoft.AspNetCore.Mvc;
using ReeferSentinel.Monolith.Data;

namespace ReeferSentinel.Monolith.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortController : ControllerBase
    {
        private readonly AppDatabase _database;

        public PortController(AppDatabase database)
        {
            _database = database;
        }

        [HttpGet(nameof(GetAllPorts))]
        public IActionResult GetAllPorts()
        {
            return Ok(_database.GetPorts());
        }
    }
}

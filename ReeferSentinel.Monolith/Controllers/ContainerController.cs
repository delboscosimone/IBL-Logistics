using Microsoft.AspNetCore.Mvc;
using ReeferSentinel.Monolith.Models;
using ReeferSentinel.Monolith.Data;

namespace ReeferSentinel.Monolith.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly AppDatabase _database;

        public ContainerController(AppDatabase database)
        {
            _database = database;
        }

        [HttpPost(nameof(InsertEmptyContainer))]
        public async Task<IActionResult> InsertEmptyContainer(string containerNumber, string AgencyCode, string agentCode, MscCategoryCode categoryCode, int? currentPortId)
        {
            if (containerNumber == null || agentCode == null)
            {
                throw new ArgumentException("One or more values are set to null.");
            }

            var containerId = await _database.CreateEmptyContainer(containerNumber, AgencyCode, agentCode, categoryCode, currentPortId: currentPortId);
            return Ok(containerId);
        }

        [HttpGet(nameof(GetAllMscCodesDetail))]
        public IActionResult GetAllMscCodesDetail()
        {
            var categories = _database.GetAllCategoryDetails();
            return Ok(categories);
        }

        [HttpGet(nameof(GetAvailableContainersForPort))]
        public IActionResult GetAvailableContainersForPort([FromQuery] int portId, [FromQuery] DateTime shippingDate)
        {
            return Ok(_database.GetAvailableContainersForPort(portId, shippingDate));
        }

        [HttpGet(nameof(GetContainerById))]
        public async Task<IActionResult> GetContainerById([FromQuery] int containerId)
        {
            var container = await _database.GetContainerById(containerId);
            if (container == null)
            {
                return NotFound($"Container with ID {containerId} not found");
            }

            return Ok(container);
        }

        [HttpGet(nameof(GetContainerProductsSummary))]
        public IActionResult GetContainerProductsSummary([FromQuery] int containerId, string AgentCode)
        {
            var summary = _database.GetContainerProductsSummary(containerId, AgentCode).Result;
            if (summary == null)
            {
                return NotFound($"Container with ID {containerId} not found or doesn't contain any products");
            }
            return Ok(summary);
        }

        [HttpGet(nameof(GetAverageTemperatureForContainer))]
        public async Task<IActionResult> GetAverageTemperatureForContainer([FromQuery] int containerId, string AgentCode)
        {
            var avgTemp = await _database.GetAverageTemperature(containerId, AgentCode);
            if (avgTemp == null)
            {
                return Ok("There are no telemetries related to this container");
            }
            return Ok(avgTemp);
        }

        [HttpGet(nameof(GetTelemetriesOutOfRange))]
        public IActionResult GetTelemetriesOutOfRange(int containerId)
        {
            List<string> telOutRange = _database.GetTelemetriesOutOfRange(containerId);
            if (telOutRange == null)
            {
                return Ok("There are no telemetries out of range");
            }
            return Ok(telOutRange);
        }

        [HttpPatch(nameof(ChangeTemperatureAndHumidity))]
        public async Task<IActionResult> ChangeTemperatureAndHumidity([FromQuery] int containerId, string AgentCode, double? newTemperatureSetpoint, double? newHumiditySetpoint)
        {
            await _database.ChangeTemperatureAndHumidity(containerId, AgentCode, newTemperatureSetpoint, newHumiditySetpoint);
            return Ok("Temperature and Humidity successfully updated");
        }
    }
}

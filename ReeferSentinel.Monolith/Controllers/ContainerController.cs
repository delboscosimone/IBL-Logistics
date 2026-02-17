using Microsoft.AspNetCore.Mvc;
using ReeferSentinel.Monolith.Models;
using ReeferSentinel.Monolith.Data;

namespace ReeferSentinel.Monolith.Controllers
{
    /// <summary>
    /// API Controller for managing refrigerated containers.
    /// Provides endpoints to create containers, add products, and retrieve information.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly AppDatabase _database;

        public ContainerController(AppDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Creates a new empty container
        /// </summary>
        /// <param name="containerNumber">Container identification number (e.g., MSCU1000)</param>
        /// <param name="AgencyCode">Shipping agency code</param>
        /// <param name="agentCode">Agent code</param>
        /// <param name="categoryCode">Type of product that will be stored</param>
        /// <returns>The ID of the newly created container</returns>
        [HttpPost(nameof(InsertEmptyContainer))]
        public async Task<IActionResult> InsertEmptyContainer(
        string containerNumber,
        string AgencyCode,
        string agentCode,
        MscCategoryCode categoryCode)
        {

            if (containerNumber == null || agentCode == null || agentCode == null)
            {
                throw new ArgumentException("One or more values are set to null.");

            }

            var containerId = await _database.CreateEmptyContainer(
                containerNumber,
                AgencyCode,
                agentCode,
                categoryCode);

            return Ok(containerId);
        }

        /// <summary> UPLOAD
        /// Gets all available product category codes
        /// </summary>
        [HttpGet(nameof(GetAllMscCodesDetail))]
        public IActionResult GetAllMscCodesDetail()
        {
            var categories = _database.GetAllCategoryDetails();
            return Ok(categories);
        }

        /// <summary>
        /// Gets complete information about a container
        /// </summary>
        /// <param name="containerId">ID of the container</param>
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

        /// <summary>
        /// Gets a summary of products in a container including space utilization
        /// </summary>
        /// <param name="containerId">ID of the container</param>
        [HttpGet(nameof(GetContainerProductsSummary))]
        public IActionResult GetContainerProductsSummary([FromQuery] int containerId, string AgentCode)
        {
            var summary = _database.GetContainerProductsSummary(containerId, AgentCode);

            if (summary == null)
            {
                return NotFound($"Container with ID {containerId} not found or doens't contain any products");
            }

            return Ok(summary);
        }

        /// <summary>
        /// Gets the average temperature for a container based on all telemetry readings
        /// </summary>
        /// <param name="containerId">ID of the container</param>
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

        /// <summary>
        /// Gets the average temperature for a container based on all telemetry readings
        /// </summary>
        /// <param name="containerId">ID of the container</param>
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
        public async Task<IActionResult> ChangeTemperatureAndHumidity([FromQuery] int containerId, string AgentCode,
                                       double? newTemperatureSetpoint, double? newHumiditySetpoint)
        {

            await _database.ChangeTemperatureAndHumidity(
                containerId,
                AgentCode,
                newTemperatureSetpoint,
                newHumiditySetpoint);

            return Ok("Temperature and Humidity successfully updated");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReeferSentinel.Monolith.Data;

namespace ReeferSentinel.Monolith.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDatabase _database;

        public BookingController(AppDatabase database)
        {
            _database = database;
        }


        /// <summary>
        /// Creates a new empty booking
        /// </summary>
        [HttpPost(nameof(CreateNewBooking))]
        public async Task<IActionResult> CreateNewBooking(
        string BkNumber,
        string CustomerName,
        string CustomerSurname,
        string CustomerTaxCode,
        string CustomerCompany,

        string ConsigneeName,
        string ConsigneeSurname,
        string ConsigneeTaxCode,
        string ConsigneeCompany,

        string OriginNation,
        string OriginCity,
        string OriginAddress,
        string OriginPostalCode,

        string DestinationNation,
        string DestinationCity,
        string DestinationAddress,
        string DestinationPostalCode,

        string agencyCode,
        DateTime ShippingDate
        )
        {

            if (BkNumber.IsNullOrEmpty() || CustomerName == null || CustomerSurname == null ||
                CustomerTaxCode == null || CustomerCompany == null ||
                ConsigneeName == null || ConsigneeSurname == null ||
                ConsigneeTaxCode == null || ConsigneeCompany == null ||
                OriginNation == null || OriginCity == null ||
                OriginAddress == null || OriginPostalCode == null ||
                DestinationNation == null || DestinationCity == null ||
                DestinationAddress == null || DestinationPostalCode == null || agencyCode == null)
            {
                throw new ArgumentException("One or more values are set to null.");

            }

            var containerId = await _database.CreateNewBooking(
                BkNumber, CustomerName, CustomerSurname, CustomerTaxCode, CustomerCompany, ConsigneeName, ConsigneeSurname, ConsigneeTaxCode, ConsigneeCompany,
                OriginNation, OriginCity, OriginAddress, OriginPostalCode, DestinationNation, DestinationCity, DestinationAddress, DestinationPostalCode,
                agencyCode, ShippingDate);

            return Ok(containerId);
        }

        /// <summary>
        /// Associate containers with booking 
        /// </summary>
        [HttpGet(nameof(AssociateContainers))]
        public async Task<IActionResult> AssociateContainers([FromQuery]
            int containerId,
            int bookingId,
            string agentCode)
        {
            var msg = await _database.AssociateContainers(containerId, bookingId, agentCode);

            if (msg == null)
            {
                return NotFound($"Container with ID {containerId} not found");
            }

            return Ok(msg);
        }

        [HttpGet(nameof(GetBookingById))]
        public IActionResult GetBookingById([FromQuery] int bookingId)
        {
            var containerList = _database.GetBookingById(bookingId);

            if (containerList == null)
            {
                return NotFound($"Booking with ID {bookingId} doesn't exist");
            }

            return Ok(containerList);
        }


        /// <summary>
        /// Gets the average temperature for a container based on all telemetry readings
        /// </summary>
        [HttpGet(nameof(GetContainerOutOfRangeByBookingId))]
        public IActionResult GetContainerOutOfRangeByBookingId([FromQuery] int bookingId)
        {
            List<string> containers = _database.GetContainerOutOfRangeByBookingId(bookingId);
            if (containers == null)
            {
                return Ok("There are no containers out of range");
            }
            return Ok(containers);
        }
    }
}

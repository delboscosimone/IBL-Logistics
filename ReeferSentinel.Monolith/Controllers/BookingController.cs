using Microsoft.AspNetCore.Mvc;
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
            DateTime ShippingDate,
            DateTime EstimatedArrivalDate,
            int? originPortId,
            int? destinationPortId)
        {
            if (string.IsNullOrWhiteSpace(BkNumber) || string.IsNullOrWhiteSpace(CustomerName))
            {
                throw new ArgumentException("One or more values are set to null.");
            }

            var bookingId = await _database.CreateNewBooking(
                BkNumber, CustomerName, CustomerSurname, CustomerTaxCode, CustomerCompany,
                ConsigneeName, ConsigneeSurname, ConsigneeTaxCode, ConsigneeCompany,
                OriginNation, OriginCity, OriginAddress, OriginPostalCode,
                DestinationNation, DestinationCity, DestinationAddress, DestinationPostalCode,
                agencyCode, ShippingDate, EstimatedArrivalDate, originPortId, destinationPortId);

            return Ok(bookingId);
        }

        [HttpGet(nameof(AssociateContainers))]
        public async Task<IActionResult> AssociateContainers([FromQuery] int containerId, int bookingId, string agentCode)
        {
            var id = await _database.AssociateContainers(containerId, bookingId, agentCode);
            return Ok(id);
        }

        [HttpGet(nameof(GetBookingById))]
        public IActionResult GetBookingById([FromQuery] int bookingId)
        {
            var booking = _database.GetBookingById(bookingId);
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} doesn't exist");
            }

            return Ok(booking);
        }

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

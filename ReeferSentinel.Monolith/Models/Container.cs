namespace ReeferSentinel.Monolith.Models
{
    /// <summary>
    /// Represents a refrigerated shipping container (reefer container).
    /// Stores information about the container, the products inside, and environmental settings.
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Maximum volume that a container can hold (in cubic meters)
        /// </summary>
        public static readonly double MAX_VOLUME = 33;

        /// <summary>
        /// Unique identifier for the container
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Container identification number (e.g., MSCU1000)
        /// </summary>
        public string ContainerNumber { get; set; }

        /// <summary>
        /// Type of product being transported
        /// </summary>
        public MscCategoryCode ProductCategory { get; set; }

        /// <summary>
        /// List of products in the container
        /// </summary>
        public List<Product> Products { get; set; }

        /// <summary>
        /// Total weight of products in kilograms
        /// </summary>
        public double TotalWeight { get; set; }

        /// <summary>
        /// Total volume of products in cubic meters
        /// </summary>
        public double TotalVolume { get; set; }

        /// <summary>
        /// List of temperature/humidity readings for this container
        /// </summary>
        public List<Telemetry> Telemetries { get; set; }

        /// <summary>
        /// Code identifying the specific agent handling this container
        /// </summary>
        public string AgentCode { get; set; }

        /// <summary>
        /// Target temperature for this container (in °C)
        /// </summary>
        public double? TemperatureSetpoint { get; set; }

        /// <summary>
        /// Target humidity for this container (in %)
        /// </summary>
        public double? HumiditySetpoint { get; set; }
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}

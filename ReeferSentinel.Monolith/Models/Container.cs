namespace ReeferSentinel.Monolith.Models
{
    public class Container
    {
        public static readonly double MAX_VOLUME = 33;

        public int Id { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public MscCategoryCode ProductCategory { get; set; }
        public List<Product> Products { get; set; } = new();
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
        public List<Telemetry> Telemetries { get; set; } = new();
        public string AgentCode { get; set; } = string.Empty;
        public double? TemperatureSetpoint { get; set; }
        public double? HumiditySetpoint { get; set; }
        public int? CurrentPortId { get; set; }
        public Port? CurrentPort { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}

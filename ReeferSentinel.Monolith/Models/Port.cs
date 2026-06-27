namespace ReeferSentinel.Monolith.Models
{
    public class Port
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public List<Container> Containers { get; set; } = new();
        public List<Booking> OriginBookings { get; set; } = new();
        public List<Booking> DestinationBookings { get; set; } = new();
    }
}

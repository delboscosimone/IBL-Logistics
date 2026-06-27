using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Data
{
    public class ContainerInfo
    {
        public int Id { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int? ProductCategoryCode { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public double? TemperatureSetpoint { get; set; }
        public double? HumiditySetpoint { get; set; }
        public int? CurrentPortId { get; set; }
        public string? CurrentPortCode { get; set; }
        public string? CurrentPortName { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ProductSummary
    {
        public string ContainerNumber { get; set; } = string.Empty;
        public string ProductCategoryName { get; set; } = string.Empty;
        public string AgencyCode { get; set; } = string.Empty;
        public string AgentCode { get; set; } = string.Empty;
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
        public double AvailableVolume { get; set; }
        public double VolumeUtilizationPercentage { get; set; }
        public double Count { get; set; }
    }

    public class SpecificProductSummary
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCategoryName { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Volume { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }

    public class SpecificContainerSummary
    {
        public int Id { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public MscCategoryCode ProductCategory { get; set; }
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
    }

    public class BookingSummary
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public string OriginAdress { get; set; } = string.Empty;
        public string DestinationAdress { get; set; } = string.Empty;
        public string? OriginPortDisplay { get; set; }
        public string? DestinationPortDisplay { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public string CustomerInformations { get; set; } = string.Empty;
        public string ConsigneeInformations { get; set; } = string.Empty;
        public List<SpecificContainerSummary> ContainerSummaries { get; set; } = new();
    }

    public class CategoryInfo
    {
        public int Code { get; set; }
        public string Description { get; set; } = string.Empty;
        public double? DefaultTemperatureSetpoint { get; set; }
        public double? DefaultHumiditySetpoint { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }

    public class PortInfo
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string DisplayName => $"{Name} ({Nation}) - {Code}";
    }
}

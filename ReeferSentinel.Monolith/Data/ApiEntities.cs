using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Data
{
// Data classes for returning information

    /// <summary>
    /// Complete information about a container
    /// </summary>
    public class ContainerInfo
    {
        public int Id { get; set; }
        public string ContainerNumber { get; set; }
        public int? ProductCategoryCode { get; set; }
        public string ProductCategoryName { get; set; }
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
        public string AgentCode { get; set; }
        public double? TemperatureSetpoint { get; set; }
        public double? HumiditySetpoint { get; set; }
    }

    /// <summary>
    /// Summary of products in a container
    /// </summary>
    public class ProductSummary
    {
        public string ContainerNumber { get; set; }
        public string ProductCategoryName { get; set; }
        public string AgencyCode { get; set; }
        public string AgentCode { get; set; }
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
        public double AvailableVolume { get; set; }
        public double VolumeUtilizationPercentage { get; set; }
        public double Count { get; set; }
    }

    public class SpecificProductSummary
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public double Weight { get; set; }
        public double Volume { get; set; }
        public string Notes { get; set; }

        public bool IsDeleted { get; set; }
    }
    public class SpecificContainerSummary
    {
        public int Id { get; set; }
        public string ContainerNumber { get; set; }
        public MscCategoryCode ProductCategory { get; set; }
        public double TotalWeight { get; set; }
        public double TotalVolume { get; set; }
    }
    public class BookingSummary
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; }
        public string OriginAdress { get; set; }
        public string DestinationAdress { get; set; }
        public string CustomerInformations { get; set; }
        public string ConsigneeInformations { get; set; }
        public List<SpecificContainerSummary> ContainerSummaries { get; set; }
    }
    /// <summary>
    /// Information about a product category
    /// </summary>
    public class CategoryInfo
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }
}

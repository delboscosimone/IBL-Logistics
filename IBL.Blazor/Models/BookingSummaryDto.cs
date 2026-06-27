namespace IBL.Blazor.Models;

public class BookingSummaryDto
{
    public int Id { get; set; }
    public string? BookingNumber { get; set; }
    public string? OriginAdress { get; set; }
    public string? DestinationAdress { get; set; }
    public string? OriginPortDisplay { get; set; }
    public string? DestinationPortDisplay { get; set; }
    public DateTime ShippingDate { get; set; }
    public DateTime EstimatedArrivalDate { get; set; }
    public string? CustomerInformations { get; set; }
    public string? ConsigneeInformations { get; set; }
    public List<SpecificContainerSummaryDto> ContainerSummaries { get; set; } = new();
}

public class SpecificContainerSummaryDto
{
    public int Id { get; set; }
    public string? ContainerNumber { get; set; }
    public int ProductCategory { get; set; }
    public double TotalWeight { get; set; }
    public double TotalVolume { get; set; }
    public string ProductCategoryLabel => MscCategoryCatalog.GetDisplayName(ProductCategory);
}

namespace IBL.Blazor.Models;

public class CreateBookingRequest
{
    public string BkNumber { get; set; } = $"BK-{DateTime.Now:yyyyMMdd-HHmm}";
    public string CustomerName { get; set; } = "Aurora";
    public string CustomerSurname { get; set; } = "Foods";
    public string CustomerTaxCode { get; set; } = "RRAFLD00A01F205X";
    public string CustomerCompany { get; set; } = "Aurora Global Foods";
    public string ConsigneeName { get; set; } = "Logistics";
    public string ConsigneeSurname { get; set; } = "Desk";
    public string ConsigneeTaxCode { get; set; } = "LGSDESK00A01F205X";
    public string ConsigneeCompany { get; set; } = "IBL Destination Desk";
    public string OriginNation { get; set; } = "ITA";
    public string OriginCity { get; set; } = "Genova";
    public string OriginAddress { get; set; } = "Terminal Sech";
    public string OriginPostalCode { get; set; } = "16126";
    public string DestinationNation { get; set; } = "USA";
    public string DestinationCity { get; set; } = "Miami";
    public string DestinationAddress { get; set; } = "Dodge Island";
    public string DestinationPostalCode { get; set; } = "33132";
    public string AgencyCode { get; set; } = "MSC-AG-2001";
    public DateTime ShippingDate { get; set; } = DateTime.Today.AddDays(1);
    public DateTime EstimatedArrivalDate { get; set; } = DateTime.Today.AddDays(9);
    public int? OriginPortId { get; set; } = 1;
    public int? DestinationPortId { get; set; } = 5;
}

namespace ReeferSentinel.Monolith.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BkNumber { get; set; } = string.Empty;
        public List<Container> Containers { get; set; } = new();

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerSurname { get; set; } = string.Empty;
        public string CustomerTaxCode { get; set; } = string.Empty;
        public string CustomerCompany { get; set; } = string.Empty;
        public string ConsigneeName { get; set; } = string.Empty;
        public string ConsigneeSurname { get; set; } = string.Empty;
        public string ConsigneeTaxCode { get; set; } = string.Empty;
        public string ConsigneeCompany { get; set; } = string.Empty;

        public string OriginNation { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string OriginAddress { get; set; } = string.Empty;
        public string OriginPostalCode { get; set; } = string.Empty;
        public string DestinationNation { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationAddress { get; set; } = string.Empty;
        public string DestinationPostalCode { get; set; } = string.Empty;

        public int? OriginPortId { get; set; }
        public Port? OriginPort { get; set; }
        public int? DestinationPortId { get; set; }
        public Port? DestinationPort { get; set; }

        public string AgencyCode { get; set; } = string.Empty;
        public DateTime ShippingDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
    }
}

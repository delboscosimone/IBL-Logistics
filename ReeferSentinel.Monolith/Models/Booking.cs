namespace ReeferSentinel.Monolith.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BkNumber { get; set; }
        public List<Container> Containers { get; set; } = new();

        // PERSON fix
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerTaxCode { get; set; }
        public string CustomerCompany { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeSurname { get; set; }
        public string ConsigneeTaxCode { get; set; }
        public string ConsigneeCompany { get; set; }

        // PLACES
        public string OriginNation { get; set; }
        public string OriginCity { get; set; }
        public string OriginAddress { get; set; }
        public string OriginPostalCode { get; set; }
        public string DestinationNation { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationPostalCode { get; set; }

        public string  AgencyCode { get; set; }
        public DateTime ShippingDate { get; set; }
    }
}

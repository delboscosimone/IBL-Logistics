namespace IBL.Blazor.Models;

public class CustomerLinkedBooking
{
    public int CustomerId { get; set; }
    public int BookingId { get; set; }
    public List<int> ContainerIds { get; set; } = new();
}

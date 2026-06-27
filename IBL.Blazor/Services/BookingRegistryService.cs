using IBL.Blazor.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace IBL.Blazor.Services;

public class BookingRegistryService
{
    private const string RegistryStorageKey = "ibl.customer-booking-links";
    private readonly ProtectedLocalStorage _storage;
    private readonly List<CustomerLinkedBooking> _links = new();

    public BookingRegistryService(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            var stored = await _storage.GetAsync<List<CustomerLinkedBooking>>(RegistryStorageKey);
            _links.Clear();
            _links.AddRange(stored.Success && stored.Value is { Count: > 0 }
                ? stored.Value
                : GetSeedLinks());
        }
        catch
        {
            _links.Clear();
            _links.AddRange(GetSeedLinks());
        }

        IsInitialized = true;
    }

    public IReadOnlyList<CustomerLinkedBooking> GetByCustomer(int customerId)
        => _links.Where(x => x.CustomerId == customerId).ToList();

    public async Task LinkBookingToCustomerAsync(int customerId, int bookingId, IEnumerable<int> containerIds)
    {
        var existing = _links.FirstOrDefault(x => x.CustomerId == customerId && x.BookingId == bookingId);
        if (existing is null)
        {
            _links.Add(new CustomerLinkedBooking
            {
                CustomerId = customerId,
                BookingId = bookingId,
                ContainerIds = containerIds.Distinct().ToList()
            });
        }
        else
        {
            existing.ContainerIds = existing.ContainerIds.Union(containerIds).Distinct().ToList();
        }

        await PersistAsync();
    }

    private async Task PersistAsync()
    {
        await _storage.SetAsync(RegistryStorageKey, _links);
    }

    private static List<CustomerLinkedBooking> GetSeedLinks() =>
    [
        new CustomerLinkedBooking { CustomerId = 3, BookingId = 1, ContainerIds = new List<int> { 1, 2 } }
    ];
}

using IBL.Blazor.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace IBL.Blazor.Services;

public class AuthStateService
{
    private const string UsersStorageKey = "ibl.auth.users";
    private const string CurrentUserStorageKey = "ibl.auth.current-user";

    private readonly ProtectedLocalStorage _storage;
    private readonly List<AppUser> _users = new();

    public AuthStateService(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public bool IsInitialized { get; private set; }
    public AppUser? CurrentUser { get; private set; }
    public event Action? OnChange;

    public IReadOnlyList<AppUser> Users => _users.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToList();

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            var storedUsers = await _storage.GetAsync<List<AppUser>>(UsersStorageKey);
            _users.Clear();
            _users.AddRange(storedUsers.Success && storedUsers.Value is { Count: > 0 }
                ? storedUsers.Value
                : GetSeedUsers());

            var currentUser = await _storage.GetAsync<AppUser>(CurrentUserStorageKey);
            CurrentUser = currentUser.Success ? currentUser.Value : null;
        }
        catch
        {
            _users.Clear();
            _users.AddRange(GetSeedUsers());
            CurrentUser = null;
        }

        IsInitialized = true;
        NotifyStateChanged();
    }

    public async Task<(bool Success, string Message)> LoginAsync(string email, string password)
    {
        var user = _users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user is null)
        {
            return (false, "Credenziali non valide.");
        }

        CurrentUser = user;
        await PersistCurrentUserAsync();
        NotifyStateChanged();
        return (true, $"Accesso effettuato come {user.RoleLabel}.");
    }

    public Task<AppUser> RegisterCustomerAsync(string fullName, string email, string password, string companyName, string taxCodeOrVatNumber)
        => CreateUserAsync(fullName, email, password, UserRole.Customer, companyName, taxCodeOrVatNumber);

    public async Task<AppUser> CreateUserAsync(string fullName, string email, string password, UserRole role, string companyName, string taxCodeOrVatNumber)
    {
        if (_users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Esiste già un account con questa email.");
        }

        var user = new AppUser
        {
            Id = (_users.Count == 0 ? 0 : _users.Max(u => u.Id)) + 1,
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Password = password,
            Role = role,
            CompanyName = companyName.Trim(),
            TaxCodeOrVatNumber = taxCodeOrVatNumber.Trim()
        };

        _users.Add(user);
        await PersistUsersAsync();
        NotifyStateChanged();
        return user;
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        try
        {
            await _storage.DeleteAsync(CurrentUserStorageKey);
        }
        catch
        {
            // ignore storage failures and still clear in-memory state
        }
        NotifyStateChanged();
    }

    private async Task PersistUsersAsync()
    {
        await _storage.SetAsync(UsersStorageKey, _users);
    }

    private async Task PersistCurrentUserAsync()
    {
        if (CurrentUser is null)
        {
            await _storage.DeleteAsync(CurrentUserStorageKey);
            return;
        }

        await _storage.SetAsync(CurrentUserStorageKey, CurrentUser);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private static List<AppUser> GetSeedUsers() =>
    [
        new AppUser
        {
            Id = 1,
            FullName = "IBL Admin",
            Email = "admin@ibl.local",
            Password = "Admin123!",
            Role = UserRole.Admin,
            CompanyName = "IBL",
            TaxCodeOrVatNumber = "IBLADMIN001"
        },
        new AppUser
        {
            Id = 2,
            FullName = "Operatore Marco",
            Email = "employee@ibl.local",
            Password = "Employee123!",
            Role = UserRole.Employee,
            CompanyName = "IBL",
            TaxCodeOrVatNumber = "IBLEMP0002"
        },
        new AppUser
        {
            Id = 3,
            FullName = "Aurora Foods",
            Email = "customer@ibl.local",
            Password = "Customer123!",
            Role = UserRole.Customer,
            CompanyName = "Aurora Global Foods",
            TaxCodeOrVatNumber = "RRAFLD00A01F205X"
        }
    ];
}

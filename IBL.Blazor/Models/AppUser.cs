using System.Text.Json.Serialization;

namespace IBL.Blazor.Models;

public enum UserRole
{
    Admin,
    Employee,
    Customer
}

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string TaxCodeOrVatNumber { get; set; } = string.Empty;

    [JsonIgnore]
    public string RoleLabel => Role switch
    {
        UserRole.Admin => "Admin",
        UserRole.Employee => "Dipendente",
        UserRole.Customer => "Cliente",
        _ => Role.ToString()
    };
}

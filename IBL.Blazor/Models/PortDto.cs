namespace IBL.Blazor.Models;

public class PortDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string DisplayName => $"{Name} ({Nation}) - {Code}";
}

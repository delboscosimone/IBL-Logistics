namespace IBL.Blazor.Models;

public static class MscCategoryCatalog
{
    public static string GetDisplayName(int code, string? fallback = null) => code switch
    {
        1 => "Pharmaceuticals and Vaccines",
        2 => "Highly Perishable Products",
        3 => "Fresh Produce",
        4 => "Frozen Products",
        5 => "Temperature Controlled Products",
        _ => string.IsNullOrWhiteSpace(fallback) ? $"Categoria {code}" : fallback
    };

    public static string GetIconUrl(int code) => code switch
    {
        1 => "/icons/pharmaceuticals.svg",
        2 => "/icons/highly-perishable.svg",
        3 => "/icons/fresh-produce.svg",
        4 => "/icons/frozen.svg",
        5 => "/icons/temperature-controlled.svg",
        _ => "/icons/container-default.svg"
    };
}

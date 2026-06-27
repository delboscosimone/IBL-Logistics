namespace IBL.Blazor.Models;

public class CategoryInfoDto
{
    public int Code { get; set; }
    public string? Description { get; set; }
    public double? DefaultTemperatureSetpoint { get; set; }
    public double? DefaultHumiditySetpoint { get; set; }
    public string? IconUrl { get; set; }

    public string DisplayName => MscCategoryCatalog.GetDisplayName(Code, Description);
    public string ResolvedIconUrl => string.IsNullOrWhiteSpace(IconUrl) ? MscCategoryCatalog.GetIconUrl(Code) : IconUrl!;
}

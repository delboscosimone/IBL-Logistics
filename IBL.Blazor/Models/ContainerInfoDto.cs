namespace IBL.Blazor.Models;

public class ContainerInfoDto
{
    public int Id { get; set; }
    public string? ContainerNumber { get; set; }
    public int? ProductCategoryCode { get; set; }
    public string? ProductCategoryName { get; set; }
    public double TotalWeight { get; set; }
    public double TotalVolume { get; set; }
    public string? AgentCode { get; set; }
    public double? TemperatureSetpoint { get; set; }
    public double? HumiditySetpoint { get; set; }
    public int? CurrentPortId { get; set; }
    public string? CurrentPortCode { get; set; }
    public string? CurrentPortName { get; set; }
    public bool IsAvailable { get; set; }
}

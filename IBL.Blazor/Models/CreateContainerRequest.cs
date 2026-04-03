namespace IBL.Blazor.Models;

public class CreateContainerRequest
{
    public string ContainerNumber { get; set; } = "TEST1234567";
    public string AgencyCode { get; set; } = "MSC-AG-2001";
    public string AgentCode { get; set; } = "AG001";
    public int CategoryCode { get; set; } = 1;
    public int CurrentPortId { get; set; } = 1;
}

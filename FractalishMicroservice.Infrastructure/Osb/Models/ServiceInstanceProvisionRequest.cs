namespace FractalishMicroservice.Infrastructure.Osb.Models;

public class ServiceInstanceProvisionRequest
{
    public string ServiceId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string OrganizationGuid { get; set; } = string.Empty;
    public string SpaceGuid { get; set; } = string.Empty;
}

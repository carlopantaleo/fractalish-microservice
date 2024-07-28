namespace FractalishMicroservice.Infrastructure.Osb.Models;

/// <summary>
/// Represents a request to provision a service instance.
/// </summary>
public class ServiceInstanceProvisionRequest
{
    /// <summary>
    /// The ID of the service to provision.
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the plan to provision.
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the organization that owns the instance.
    /// </summary>
    public string OrganizationGuid { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the space where the instance will be provisioned.
    /// </summary>
    public string SpaceGuid { get; set; } = string.Empty;
}

namespace FractalishMicroservice.Api.Models;

/// <summary>
/// Represents the configuration settings for the service catalog.
/// </summary>
public class CatalogConfiguration
{
    /// <summary>
    /// The list of service offerings in the catalog.
    /// </summary>
    public List<ServiceOffering> Services { get; set; } = [];
}

/// <summary>
/// Represents a service plan for a service offering.
/// </summary>
public class ServicePlan
{
    /// <summary>
    /// The ID of the service plan.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the service plan.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the service plan.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents a service offering in the service catalog.
/// </summary>
public class ServiceOffering
{
    /// <summary>
    /// The ID of the service offering.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the service offering.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the service offering.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Specifies whether Service Instances of the service can be bound to applications
    /// </summary>
    public bool Bindable { get; set; }

    /// <summary>
    /// The list of service plans available for the service offering.
    /// </summary>
    public List<ServicePlan> Plans { get; set; } = [];
}

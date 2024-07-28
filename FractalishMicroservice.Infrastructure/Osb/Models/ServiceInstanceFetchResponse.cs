namespace FractalishMicroservice.Infrastructure.Osb.Models;

/// <summary>
/// Represents a response to a service instance fetch request.
/// </summary>
public class ServiceInstanceFetchResponse
{
    /// <summary>
    /// The ID of the service instance.
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the service plan.
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// Optional parameters of the service instance.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();

    // TODO:  add additional optional properties
}

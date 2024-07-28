namespace FractalishMicroservice.Infrastructure.Osb.Models;

/// <summary>
/// Represents a response to a service instance provision request.
/// </summary>
public class ServiceInstanceProvisionResponse
{
    /// <summary>
    /// The ID of the operation in progress.
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    // TODO: add additional optional properties
}

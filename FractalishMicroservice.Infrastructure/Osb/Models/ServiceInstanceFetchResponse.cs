namespace FractalishMicroservice.Infrastructure.Osb.Models;

public class ServiceInstanceFetchResponse
{
    public string ServiceId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();

    // TODO:  add additional optional properties
}

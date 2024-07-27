namespace FractalishMicroservice.Api.Models;

public class CatalogConfiguration
{
    public List<ServiceOffering> Services { get; set; } = [];
}

public class ServicePlan
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ServiceOffering
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Bindable { get; set; }
    public List<ServicePlan> Plans { get; set; } = [];
}

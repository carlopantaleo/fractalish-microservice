using FractalishMicroservice.Infrastructure.Osb.Models;

namespace FractalishMicroservice.Infrastructure.Osb;

public interface IOsbService
{
    Task<ServiceInstanceProvisionResponse> ProvisionServiceInstanceAsync(string instanceId,
                                                                         ServiceInstanceProvisionRequest request);

    Task DeprovisionServiceInstanceAsync(string instanceId);
    Task<ServiceInstanceFetchResponse> FetchServiceInstanceAsync(string instanceId);
}

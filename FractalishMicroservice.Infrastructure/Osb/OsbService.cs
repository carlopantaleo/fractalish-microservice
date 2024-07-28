using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Infrastructure.Osb.Models;

namespace FractalishMicroservice.Infrastructure.Osb;

public class OsbService : IOsbService
{
    private readonly IVmInstanceService _vmInstanceService;

    // TODO: now this service handles only VMs, other types of services may be handled. It may be further implemented
    // to access the service catalog, retrieve the service type and use the appropriate service to handle operations
    // (other services like IK8sInstanceService, IFunctionsService and so on).

    public OsbService(IVmInstanceService vmInstanceService)
    {
        _vmInstanceService = vmInstanceService;
    }

    public async Task<ServiceInstanceProvisionResponse> ProvisionServiceInstanceAsync(string instanceId,
                                                                                      ServiceInstanceProvisionRequest request)
    {
        string vmInstanceId = await _vmInstanceService.CreateVmInstance(request.PlanId, request.ServiceId);

        // We return the vmInstanceId as the Operation. In a real-world scenario a more robust approach (like
        // correlating the passed instanceId to the vmInstanceId and persisting it on a database) may be implemented.
        return new ServiceInstanceProvisionResponse
        {
            Operation = vmInstanceId
        };
    }

    public async Task DeprovisionServiceInstanceAsync(string instanceId)
    {
        await _vmInstanceService.TerminateVmInstance(instanceId);
    }

    public async Task<ServiceInstanceFetchResponse> FetchServiceInstanceAsync(string instanceId)
    {
        var state = await _vmInstanceService.GetVmInstanceState(instanceId);
        return new ServiceInstanceFetchResponse
        {
            ServiceId = "example-service-id", // You would typically get this from your data store
            PlanId = "example-plan-id", // You would typically get this from your data store
            Parameters = new Dictionary<string, string> { { "state", state.ToString() } }
        };
    }
}

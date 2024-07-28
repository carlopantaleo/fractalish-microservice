using FractalishMicroservice.Infrastructure.Osb;
using FractalishMicroservice.Infrastructure.Osb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FractalishMicroservice.Api.Controllers;

/// <summary>
/// This controller handles provisioning, deprovisioning and fetching of a service instance.
/// </summary>
[Route("v2/service_instances/{instanceId}")]
[ApiController]
public class ServiceInstancesController : ControllerBase
{
    private readonly IOsbService _osbService;

    public ServiceInstancesController(IOsbService osbService)
    {
        _osbService = osbService;
    }

    [HttpPut]
    public async Task<IActionResult> ProvisionServiceInstance(string instanceId,
                                                              [FromBody] ServiceInstanceProvisionRequest request)
    {
        var response = await _osbService.ProvisionServiceInstanceAsync(instanceId, request);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeprovisionServiceInstance(string instanceId)
    {
        await _osbService.DeprovisionServiceInstanceAsync(instanceId);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> FetchServiceInstance(string instanceId)
    {
        var response = await _osbService.FetchServiceInstanceAsync(instanceId);
        return Ok(response);
    }
}

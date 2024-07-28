using FractalishMicroservice.Infrastructure.Osb;
using FractalishMicroservice.Infrastructure.Osb.Models;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Provisions a service instance.
    /// </summary>
    /// <param name="instanceId">The ID of the service instance to provision.</param>
    /// <param name="request">The service instance provision request.</param>
    /// <response code="200">The service instance was provisioned successfully.</response>
    [HttpPut]
    [ProducesResponseType(typeof(ServiceInstanceProvisionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProvisionServiceInstance(string instanceId,
                                                              [FromBody] ServiceInstanceProvisionRequest request)
    {
        var response = await _osbService.ProvisionServiceInstanceAsync(instanceId, request);
        return Ok(response);
    }

    /// <summary>
    /// Deprovisions a service instance.
    /// </summary>
    /// <param name="instanceId">The ID of the service instance to deprovision.</param>
    /// <response code="200">The service instance was deprovisioned successfully.</response>
    [HttpDelete]
    public async Task<IActionResult> DeprovisionServiceInstance(string instanceId)
    {
        await _osbService.DeprovisionServiceInstanceAsync(instanceId);
        return NoContent();
    }

    /// <summary>
    /// Fetches a service instance.
    /// </summary>
    /// <param name="instanceId">The ID of the service instance to fetch.</param>
    /// <response code="200">The service instance was fetched successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceInstanceFetchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> FetchServiceInstance(string instanceId)
    {
        var response = await _osbService.FetchServiceInstanceAsync(instanceId);
        return Ok(response);
    }
}

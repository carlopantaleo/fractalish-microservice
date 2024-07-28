using FractalishMicroservice.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FractalishMicroservice.Api.Controllers;

/// <summary>
/// Provides endpoints for retrieving service catalog information.
/// </summary>
[ApiController]
[Route("v2/catalog")]
public class CatalogController : ControllerBase
{
    private readonly CatalogConfiguration _catalogConfig;

    public CatalogController(IOptions<CatalogConfiguration> catalogConfig)
    {
        _catalogConfig = catalogConfig.Value;
    }

    /// <summary>
    /// Gets the service catalog.
    /// </summary>
    /// <returns>The service catalog.</returns>
    [HttpGet]
    public IActionResult GetCatalog()
    {
        return Ok(new GetCatalogResponse
        {
            Services = _catalogConfig.Services
        });
    }

    /// <summary>
    /// Represents the response for GET /v2/catalog endpoint.
    /// </summary>
    public class GetCatalogResponse
    {
        /// <summary>
        /// The list of service offerings.
        /// </summary>
        public IEnumerable<ServiceOffering> Services { get; set; } = [];
    }
}

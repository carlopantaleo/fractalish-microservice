using FractalishMicroservice.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FractalishMicroservice.Api.Controllers;

[ApiController]
[Route("v2/catalog")]
public class CatalogController : ControllerBase
{
    private readonly CatalogConfiguration _catalogConfig;

    public CatalogController(IOptions<CatalogConfiguration> catalogConfig)
    {
        _catalogConfig = catalogConfig.Value;
    }

    [HttpGet]
    public IActionResult GetCatalog()
    {
        return Ok(new GetCatalogResponse
        {
            Services = _catalogConfig.Services
        });
    }

    public class GetCatalogResponse
    {
        public IEnumerable<ServiceOffering> Services { get; set; } = [];
    }
}

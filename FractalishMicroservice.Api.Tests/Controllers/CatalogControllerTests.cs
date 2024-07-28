using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Api.Controllers;
using FractalishMicroservice.Api.Models;
using FractalishMicroservice.Tests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FractalishMicroservice.Api.Tests.Controllers;

public class CatalogControllerTests : TestBase
{
    private readonly CatalogConfiguration _mockCatalogConfiguration;
    private readonly CatalogController _sut;

    public CatalogControllerTests()
    {
        _mockCatalogConfiguration = new CatalogConfiguration();
        var mockCatalogConfigurationOptions = _mockRepository.Create<IOptions<CatalogConfiguration>>();
        mockCatalogConfigurationOptions
            .SetupGet(x => x.Value)
            .Returns(_mockCatalogConfiguration);
        _sut = new CatalogController(mockCatalogConfigurationOptions.Object);
    }

    [Fact]
    public void GetCatalog_ShouldReturnOkResultWithServices()
    {
        // Arrange
        var services = _fixture.CreateMany<ServiceOffering>(2).ToList();
        _mockCatalogConfiguration.Services = services;

        // Act
        var result = _sut.GetCatalog();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<CatalogController.GetCatalogResponse>().Subject;
        value.Services.Should().BeEquivalentTo(services);

        VerifyAll();
    }

    [Fact]
    public void GetCatalog_NoServicesConfigured_ReturnEmptyList()
    {
        // Act
        var result = _sut.GetCatalog();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<CatalogController.GetCatalogResponse>().Subject;
        value.Services.Should().BeEmpty();
    }
}

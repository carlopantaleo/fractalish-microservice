using System.Net;
using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Api.Controllers;
using FractalishMicroservice.Infrastructure.Osb;
using FractalishMicroservice.Infrastructure.Osb.Models;
using FractalishMicroservice.Tests.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FractalishMicroservice.Api.Tests.Controllers;

public class ServiceInstancesControllerTests : TestBase
{
    private readonly Mock<IOsbService> _osbServiceMock;
    private readonly ServiceInstancesController _sut;
    private readonly string _instanceId;

    public ServiceInstancesControllerTests()
    {
        _osbServiceMock = _mockRepository.Create<IOsbService>();
        _sut = new ServiceInstancesController(_osbServiceMock.Object);
        _instanceId = _fixture.Create<string>();
    }

    [Fact]
    public async Task ProvisionServiceInstance_ValidRequest_ReturnsOkResultWithProvisionResponse()
    {
        // Arrange
        var request = _fixture.Create<ServiceInstanceProvisionRequest>();
        var provisionResponse = _fixture.Create<ServiceInstanceProvisionResponse>();
        _osbServiceMock
            .Setup(x => x.ProvisionServiceInstanceAsync(_instanceId, request))
            .ReturnsAsync(provisionResponse);

        // Act
        var response = await _sut.ProvisionServiceInstance(_instanceId, request);

        // Assert
        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(provisionResponse);
        VerifyAll();
    }

    [Fact]
    public async Task DeprovisionServiceInstance_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        _osbServiceMock
            .Setup(x => x.DeprovisionServiceInstanceAsync(_instanceId))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _sut.DeprovisionServiceInstance(_instanceId);

        // Assert
        response.Should().BeOfType<NoContentResult>();
        VerifyAll();
    }

    [Fact]
    public async Task FetchServiceInstance_ValidRequest_ReturnsOkResultWithFetchResponse()
    {
        // Arrange
        var fetchResponse = _fixture.Create<ServiceInstanceFetchResponse>();
        _osbServiceMock
            .Setup(x => x.FetchServiceInstanceAsync(_instanceId))
            .ReturnsAsync(fetchResponse);

        // Act
        var response = await _sut.FetchServiceInstance(_instanceId);

        // Assert
        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(fetchResponse);
        VerifyAll();
    }
}

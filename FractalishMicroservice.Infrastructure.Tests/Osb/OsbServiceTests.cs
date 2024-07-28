using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Infrastructure.Osb;
using FractalishMicroservice.Infrastructure.Osb.Models;
using FractalishMicroservice.Tests.Common;
using Moq;

namespace FractalishMicroservice.Infrastructure.Tests.Osb;

public class OsbServiceTests : TestBase
{
    private readonly Mock<IVmInstanceService> _vmInstanceServiceMock;
    private readonly OsbService _sut;
    private readonly string _instanceId;
    private readonly string _serviceId;
    private readonly string _planId;

    public OsbServiceTests()
    {
        _vmInstanceServiceMock = _mockRepository.Create<IVmInstanceService>();
        _sut = new OsbService(_vmInstanceServiceMock.Object);
        _instanceId = _fixture.Create<string>();
        _serviceId = _fixture.Create<string>();
        _planId = _fixture.Create<string>();
    }

    [Fact]
    public async Task ProvisionServiceInstanceAsync_ValidRequest_ProvisionsInstance()
    {
        // Arrange
        var request = new ServiceInstanceProvisionRequest
        {
            ServiceId = _serviceId,
            PlanId = _planId
        };
        var createdVmInstanceId = _fixture.Create<string>();

        _vmInstanceServiceMock
            .Setup(x => x.CreateVmInstance(request.PlanId, request.ServiceId))
            .ReturnsAsync(createdVmInstanceId);

        // Act
        var response = await _sut.ProvisionServiceInstanceAsync(_instanceId, request);

        // Assert
        response.Should().NotBeNull();
        response.Operation.Should().Be(createdVmInstanceId);
        VerifyAll();
    }

    [Fact]
    public async Task DeprovisionServiceInstanceAsync_ValidInstanceId_CallsTerminateVmInstance()
    {
        // Arrange
        _vmInstanceServiceMock
            .Setup(x => x.TerminateVmInstance(_instanceId))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeprovisionServiceInstanceAsync(_instanceId);

        // Assert
        VerifyAll();
    }

    [Fact]
    public async Task FetchServiceInstanceAsync_ValidInstanceId_ReturnsServiceInstanceFetchResponse()
    {
        // Arrange
        var vmInstanceState = _fixture.Create<VmInstanceState>();

        _vmInstanceServiceMock
            .Setup(x => x.GetVmInstanceState(_instanceId))
            .ReturnsAsync(vmInstanceState);

        // Act
        var response = await _sut.FetchServiceInstanceAsync(_instanceId);

        // Assert
        response.Should().NotBeNull();
        response.ServiceId.Should().Be("example-service-id");
        response.PlanId.Should().Be("example-plan-id");
        response.Parameters.Should().ContainKey("state").WhoseValue.Should().Be(vmInstanceState.ToString());
        VerifyAll();
    }
}

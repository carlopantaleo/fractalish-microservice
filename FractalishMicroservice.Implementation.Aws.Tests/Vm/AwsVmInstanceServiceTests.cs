using Amazon.EC2;
using Amazon.EC2.Model;
using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Implementation.Aws.Vm;
using FractalishMicroservice.Tests.Common;
using Moq;

namespace FractalishMicroservice.Implementation.Aws.Tests.Vm;

public sealed class AwsVmInstanceServiceTests : TestBase, IDisposable
{
    private readonly Mock<IAmazonEC2> _ec2ClientMock;
    private readonly AwsVmInstanceService _sut;

    public AwsVmInstanceServiceTests()
    {
        _ec2ClientMock = _mockRepository.Create<IAmazonEC2>();
        _sut = new AwsVmInstanceService(_ec2ClientMock.Object);
    }

    [Fact]
    public async Task CreateVmInstance_ShouldCallRunInstancesAsync_AndReturnInstanceId()
    {
        // Arrange
        var instanceType = _fixture.Create<string>();
        var amiId = _fixture.Create<string>();
        var instanceId = _fixture.Create<string>();

        _ec2ClientMock
            .Setup(x => x.RunInstancesAsync(It.Is<RunInstancesRequest>(request =>
                    request.ImageId == amiId &&
                    request.InstanceType == instanceType &&
                    request.MinCount == 1 &&
                    request.MaxCount == 1),
                default))
            .ReturnsAsync(new RunInstancesResponse
            {
                Reservation = new Reservation
                {
                    Instances = [new Instance { InstanceId = instanceId }]
                }
            });

        // Act
        var result = await _sut.CreateVmInstance(instanceType, amiId);

        // Assert
        result.Should().Be(instanceId);
        VerifyAll();
    }

    [Fact]
    public async Task TerminateVmInstance_ShouldCallTerminateInstancesAsync()
    {
        // Arrange
        var instanceId = _fixture.Create<string>();

        _ec2ClientMock
            .Setup(x => x.TerminateInstancesAsync(It.Is<TerminateInstancesRequest>(request =>
                    request.InstanceIds[0] == instanceId),
                default))
            .ReturnsAsync((TerminateInstancesResponse)null!);

        // Act
        await _sut.TerminateVmInstance(instanceId);

        // Assert
        VerifyAll();
    }

    [Fact]
    public async Task GetVmInstanceState_ShouldCallDescribeInstancesAsync_AndReturnVmInstanceState()
    {
        // Arrange
        var instanceId = _fixture.Create<string>();
        const VmInstanceState instanceState = VmInstanceState.Pending;
        const int awsInstanceStateCode = 0;

        _ec2ClientMock
            .Setup(x => x.DescribeInstancesAsync(It.Is<DescribeInstancesRequest>(request =>
                    request.InstanceIds[0] == instanceId),
                default))
            .ReturnsAsync(new DescribeInstancesResponse
            {
                Reservations =
                [
                    new Reservation
                    {
                        Instances = [new Instance { State = new InstanceState { Code = awsInstanceStateCode } }]
                    }
                ]
            });

        // Act
        var result = await _sut.GetVmInstanceState(instanceId);

        // Assert
        result.Should().Be(instanceState);
        VerifyAll();
    }

    [Fact]
    public void Dispose_ShouldDisposeEc2Client()
    {
        // Arrange
        _ec2ClientMock.Setup(x => x.Dispose());

        // Act
        _sut.Dispose();

        // Assert
        VerifyAll();
    }

    public void Dispose()
    {
        _ec2ClientMock.Setup(x => x.Dispose());
        _sut.Dispose();
    }
}

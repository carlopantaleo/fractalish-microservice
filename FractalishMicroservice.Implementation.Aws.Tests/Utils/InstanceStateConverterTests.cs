using Amazon.EC2.Model;
using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Implementation.Aws.Utils;
using FractalishMicroservice.Tests.Common;

namespace FractalishMicroservice.Implementation.Aws.Tests.Utils;

public class InstanceStateConverterTests : TestBase
{
    [Fact]
    public void ToVmInstanceState_NullInstanceState_ThrowsArgumentNullException()
    {
        // Arrange
        InstanceState instanceState = null!;

        // Act
        Action act = () => instanceState.ToVmInstanceState();

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*'instanceState'*");
        VerifyAll();
    }

    [Theory]
    [InlineData(0, VmInstanceState.Pending)]
    [InlineData(16, VmInstanceState.Running)]
    [InlineData(32, VmInstanceState.ShuttingDown)]
    [InlineData(48, VmInstanceState.Terminated)]
    [InlineData(64, VmInstanceState.Stopping)]
    [InlineData(80, VmInstanceState.Stopped)]
    public void ToVmInstanceState_ValidCode_ReturnsCorrectVmInstanceState(int code, VmInstanceState expectedVmInstanceState)
    {
        // Arrange
        var instanceState = _fixture.Build<InstanceState>()
            .With(x => x.Code, code)
            .Create();

        // Act
        var vmInstanceState = instanceState.ToVmInstanceState();

        // Assert
        vmInstanceState.Should().Be(expectedVmInstanceState);
        VerifyAll();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(17)]
    [InlineData(128)]
    [InlineData(255)]
    public void ToVmInstanceState_InvalidCode_ThrowsArgumentException(int code)
    {
        // Arrange
        var instanceState = _fixture.Build<InstanceState>()
            .With(x => x.Code, code)
            .Create();

        // Act
        Action act = () => instanceState.ToVmInstanceState();

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
            .WithMessage($"Invalid InstanceState.Code value: {code} (Parameter 'instanceState')");
        VerifyAll();
    }
}

using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Exceptions;
using FractalishMicroservice.Tests.Common;

namespace FractalishMicroservice.Abstractions.Tests.Exceptions;

public class InvalidConfigurationExceptionTests : TestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ThrowIfNullOrEmpty_NullOrEmptyValue_ThrowsInvalidConfigurationException(string value)
    {
        // Arrange
        var message = _fixture.Create<string>();

        // Act
        var act = () => InvalidConfigurationException.ThrowIfNullOrWhitespace(value, message);

        // Assert
        act.Should()
            .ThrowExactly<InvalidConfigurationException>()
            .WithMessage(message);
        VerifyAll();
    }

    [Fact]
    public void ThrowIfNullOrEmpty_ValidValue_DoesNotThrow()
    {
        // Arrange
        var value = _fixture.Create<string>();
        var message = _fixture.Create<string>();

        // Act
        var act = () => InvalidConfigurationException.ThrowIfNullOrWhitespace(value, message);

        // Assert
        act.Should().NotThrow();
        VerifyAll();
    }
}

using AutoFixture;
using FluentAssertions;
using Moq;

namespace FractalishMicroservice.Tests.Common;

/// <summary>
/// Base class to be used for all unit tests.
/// </summary>
public abstract class TestBase {
    protected readonly MockRepository _mockRepository = new(MockBehavior.Strict);
    protected readonly Fixture _fixture = new();

    protected void VerifyAll() {
        var assert = () => _mockRepository.VerifyAll();
        assert.Should().NotThrow();
    }
}

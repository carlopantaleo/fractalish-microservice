using System.Net;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Exceptions;
using FractalishMicroservice.Infrastructure.Middlewares;
using FractalishMicroservice.Tests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;

namespace FractalishMicroservice.Infrastructure.Tests.Middlewares;

public sealed class ExceptionHandlerMiddlewareTests : TestBase, IDisposable, IAsyncDisposable
{
    private readonly Mock<IHostEnvironment> _envMock;
    private readonly ExceptionHandlerMiddleware _sut;
    private readonly DefaultHttpContext _httpContext;
    private readonly MemoryStream _bodyStream;
    private readonly Mock<RequestDelegate> _requestDelegateMock;

    public ExceptionHandlerMiddlewareTests()
    {
        _envMock = _mockRepository.Create<IHostEnvironment>();
        _requestDelegateMock = _mockRepository.Create<RequestDelegate>();
        _sut = new ExceptionHandlerMiddleware(_requestDelegateMock.Object, _envMock.Object);
        _bodyStream = new MemoryStream();
        _httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = _bodyStream
            }
        };
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextMiddleware()
    {
        // Arrange
        _requestDelegateMock
            .Setup(x => x(_httpContext))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(_httpContext);

        // Assert
        _requestDelegateMock.Verify(x => x(_httpContext), Times.Once);
        VerifyAll();
    }

    [Fact]
    public async Task InvokeAsync_ServiceInstanceExceptionDevelopmentEnv_ReturnsCorrectStatusCodeAndMessage()
    {
        // Arrange
        _envMock.SetupGet(x => x.EnvironmentName).Returns("Development");
        var statusCode = HttpStatusCode.BadRequest;
        var message = _fixture.Create<string>();
        var innerMessage = _fixture.Create<string>();
        var exception = new ServiceInstanceException(statusCode, message, new Exception(innerMessage));

        _requestDelegateMock
            .Setup(x => x(_httpContext))
            .ThrowsAsync(exception);

        // Act
        await _sut.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)statusCode);

        var problemDetails = await GetProblemDetailsFromResponse();

        problemDetails.Status.Should().Be((int)statusCode);
        problemDetails.Title.Should().Be(message);
        problemDetails.Detail.Should().Be(innerMessage);

        VerifyAll();
    }

    [Fact]
    public async Task InvokeAsync_ServiceInstanceExceptionProductionEnv_ReturnsCorrectStatusCodeAndMessage()
    {
        // Arrange
        _envMock.SetupGet(x => x.EnvironmentName).Returns("Production");
        var statusCode = HttpStatusCode.BadRequest;
        var message = _fixture.Create<string>();
        var innerMessage = _fixture.Create<string>();
        var exception = new ServiceInstanceException(statusCode, message, new Exception(innerMessage));

        _requestDelegateMock
            .Setup(x => x(_httpContext))
            .ThrowsAsync(exception);

        // Act
        await _sut.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)statusCode);

        var problemDetails = await GetProblemDetailsFromResponse();

        problemDetails.Status.Should().Be((int)statusCode);
        problemDetails.Title.Should().Be(message);
        problemDetails.Detail.Should().BeNull();

        VerifyAll();
    }

    [Fact]
    public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
    {
        // Arrange
        _envMock.SetupGet(x => x.EnvironmentName).Returns("Development");
        var exception = new Exception(_fixture.Create<string>());
        _requestDelegateMock
            .Setup(x => x(_httpContext))
            .ThrowsAsync(exception);

        // Act
        await _sut.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        var problemDetails = await GetProblemDetailsFromResponse();

        problemDetails.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        problemDetails.Title.Should().Be("An unexpected error occurred.");

        VerifyAll();
    }

    private async Task<ProblemDetails> GetProblemDetailsFromResponse()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        return JsonSerializer.Deserialize<ProblemDetails>(responseContent)!;
    }

    public void Dispose()
    {
        _bodyStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _bodyStream.DisposeAsync();
    }
}

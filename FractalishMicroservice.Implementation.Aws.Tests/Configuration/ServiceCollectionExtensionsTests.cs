using Amazon.EC2;
using AutoFixture;
using FluentAssertions;
using FractalishMicroservice.Abstractions.Exceptions;
using FractalishMicroservice.Implementation.Aws.Configuration;
using FractalishMicroservice.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FractalishMicroservice.Implementation.Aws.Tests.Configuration;

public class ServiceCollectionExtensionsTests : TestBase
{
    [Fact]
    public void AddAwsServices_ValidConfiguration_AddsAmazonEC2Client()
    {
        // Arrange
        var services = new ServiceCollection();
        var awsConfiguration = _fixture.Build<AwsConfiguration>()
            .With(x => x.AccessKey, _fixture.Create<string>())
            .With(x => x.SecretKey, _fixture.Create<string>())
            .With(x => x.Region, _fixture.Create<string>())
            .Create();
        services.AddOptions<AwsConfiguration>().Configure(o =>
        {
            o.AccessKey = awsConfiguration.AccessKey;
            o.SecretKey = awsConfiguration.SecretKey;
            o.Region = awsConfiguration.Region;
        });

        // Act
        services.AddAwsServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<IAmazonEC2>().Should().NotBeNull();
        VerifyAll();
    }

    [Theory]
    [InlineData(null, "testSecretKey", "testRegion")]
    [InlineData("testAccessKey", null, "testRegion")]
    [InlineData("testAccessKey", "testSecretKey", null)]
    [InlineData("", "testSecretKey", "testRegion")]
    [InlineData("testAccessKey", "", "testRegion")]
    [InlineData("testAccessKey", "testSecretKey", "")]
    public void AddAwsServices_NullOrEmptyAWSCredentials_ThrowsInvalidConfigurationException(string accessKey, string secretKey, string region)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOptions<AwsConfiguration>().Configure(o =>
        {
            o.AccessKey = accessKey;
            o.SecretKey = secretKey;
            o.Region = region;
        });
        services.AddAwsServices();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var act = () => serviceProvider.GetRequiredService<IAmazonEC2>();

        // Assert
        act.Should().Throw<InvalidConfigurationException>();
        VerifyAll();
    }
}

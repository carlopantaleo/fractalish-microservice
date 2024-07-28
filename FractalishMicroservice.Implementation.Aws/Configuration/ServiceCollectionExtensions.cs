using Amazon.EC2;
using FractalishMicroservice.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FractalishMicroservice.Implementation.Aws.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds required AWS services to the DI container.
    /// </summary>
    public static IServiceCollection AddAwsServices(this IServiceCollection services) =>
        services.AddScoped<IAmazonEC2>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<AwsConfiguration>>().Value;
            InvalidConfigurationException.ThrowIfNullOrWhitespace(config.AccessKey, "AwsConfiguration.AccessKey is not set.");
            InvalidConfigurationException.ThrowIfNullOrWhitespace(config.SecretKey, "AwsConfiguration.SecretKey is not set.");
            InvalidConfigurationException.ThrowIfNullOrWhitespace(config.Region, "AwsConfiguration.Region is not set.");

            return new AmazonEC2Client(config.AccessKey, config.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(config.Region));
        });
}

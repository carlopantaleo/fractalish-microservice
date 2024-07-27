using Amazon.EC2;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Implementation.Aws.Configuration;
using Microsoft.Extensions.Options;

namespace FractalishMicroservice.Implementation.Aws.Vm;

public class AwsVmInstanceService : IVmInstanceService
{
    private readonly IAmazonEC2 _ec2Client;

    public AwsVmInstanceService(IOptions<AwsConfiguration> awsConfig)
    {
        var config = awsConfig.Value;
        _ec2Client = new AmazonEC2Client(config.AccessKey, config.SecretKey, Amazon.RegionEndpoint.GetBySystemName(config.Region));
    }

    public async Task<string> CreateVmInstance(string instanceType, string amiId) {
        throw new NotImplementedException();
    }

    public async Task TerminateVmInstance(string instanceId)
    {
        throw new NotImplementedException();
    }

    public async Task<VmInstanceState> GetVmInstanceState(string instanceId)
    {
        throw new NotImplementedException();
    }
}

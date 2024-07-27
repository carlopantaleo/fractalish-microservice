using Amazon.EC2;
using Amazon.EC2.Model;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Implementation.Aws.Utils;

namespace FractalishMicroservice.Implementation.Aws.Vm;

public sealed class AwsVmInstanceService : IVmInstanceService, IDisposable
{
    private readonly IAmazonEC2 _ec2Client;

    public AwsVmInstanceService(IAmazonEC2 ec2Client)
    {
        _ec2Client = ec2Client;
    }

    public async Task<string> CreateVmInstance(string instanceType, string imageId)
    {
        var request = new RunInstancesRequest
        {
            ImageId = imageId,
            InstanceType = instanceType,
            MinCount = 1,
            MaxCount = 1
        };

        var response = await _ec2Client.RunInstancesAsync(request);
        return response.Reservation.Instances[0].InstanceId;
    }

    public async Task TerminateVmInstance(string instanceId)
    {
        var request = new TerminateInstancesRequest
        {
            InstanceIds = [instanceId]
        };

        await _ec2Client.TerminateInstancesAsync(request);
    }

    public async Task<VmInstanceState> GetVmInstanceState(string instanceId)
    {
        var request = new DescribeInstancesRequest
        {
            InstanceIds = [instanceId]
        };

        var response = await _ec2Client.DescribeInstancesAsync(request);
        return response.Reservations[0].Instances[0].State.ToVmInstanceState();
    }

    public void Dispose()
    {
        _ec2Client.Dispose();
    }
}

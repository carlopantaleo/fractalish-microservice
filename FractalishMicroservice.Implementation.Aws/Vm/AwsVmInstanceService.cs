using System.Net;
using Amazon.EC2;
using Amazon.EC2.Model;
using FractalishMicroservice.Abstractions.Exceptions;
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

        try
        {
            var response = await _ec2Client.RunInstancesAsync(request);
            return response.Reservation.Instances[0].InstanceId;
        }
        catch (AmazonEC2Exception ex)
        {
            throw new ServiceInstanceException(ex.StatusCode, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ServiceInstanceException(HttpStatusCode.InternalServerError, "Error creating VM instance.", ex);
        }
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

        try
        {
            var response = await _ec2Client.DescribeInstancesAsync(request);

            var instancesCount = response?.Reservations?.FirstOrDefault()?.Instances?.Count ?? 0;
            if (instancesCount == 0)
            {
                throw new ServiceInstanceException(HttpStatusCode.NotFound,
                    $"No VM instance found with ID: {instanceId}");
            }

            return response!.Reservations![0].Instances[0].State.ToVmInstanceState();
        }
        catch (AmazonEC2Exception ex)
        {
            throw new ServiceInstanceException(ex.StatusCode, ex.Message, ex);
        }
        catch (Exception ex) when (ex is not ServiceInstanceException)
        {
            throw new ServiceInstanceException(HttpStatusCode.InternalServerError,
                "Error retrieving VM instance state.", ex);
        }
    }

    public void Dispose()
    {
        _ec2Client.Dispose();
    }
}

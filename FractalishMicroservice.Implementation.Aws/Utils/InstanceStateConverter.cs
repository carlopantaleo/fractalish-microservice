using Amazon.EC2.Model;
using FractalishMicroservice.Abstractions.Vm;

namespace FractalishMicroservice.Implementation.Aws.Utils;

public static class InstanceStateConverter
{
    /// <summary>
    /// Converts a EC2 <see cref="InstanceState"/> to a <see cref="VmInstanceState"/>.
    /// </summary>
    /// <param name="instanceState">The <see cref="InstanceState"/> to be converted.</param>
    /// <returns>A <see cref="VmInstanceState"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="instanceState"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="instanceState"/> has an invalid
    /// <see cref="InstanceState.Code"/>.</exception>
    /// <remarks>This method is based on AWS specification for <see cref="InstanceState.Code"/></remarks>
    public static VmInstanceState ToVmInstanceState(this InstanceState instanceState)
    {
        ArgumentNullException.ThrowIfNull(instanceState);

        // Extract the low byte value of the code.
        var lowByteCode = instanceState.Code & 0xFF;

        return lowByteCode switch
        {
            0 => VmInstanceState.Pending,
            16 => VmInstanceState.Running,
            32 => VmInstanceState.ShuttingDown,
            48 => VmInstanceState.Terminated,
            64 => VmInstanceState.Stopping,
            80 => VmInstanceState.Stopped,
            _ => throw new ArgumentException($"Invalid InstanceState.Code value: {lowByteCode}", nameof(instanceState)),
        };
    }
}

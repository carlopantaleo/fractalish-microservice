namespace FractalishMicroservice.Abstractions.Vm;

/// <summary>
/// Provides methods for interacting with virtual machine (VM) instances.
/// </summary>
public interface IVmInstanceService
{
    /// <summary>
    /// Creates a new VM instance.
    /// </summary>
    /// <param name="instanceType">The type of the VM instance to create.</param>
    /// <param name="imageId">The ID of the image to use for the VM instance.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains the ID of the created VM instance.</returns>
    Task<string> CreateVmInstance(string instanceType, string imageId);

    /// <summary>
    /// Terminates a VM instance.
    /// </summary>
    /// <param name="instanceId">The ID of the VM instance to terminate.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task TerminateVmInstance(string instanceId);

    /// <summary>
    /// Gets the state of a VM instance.
    /// </summary>
    /// <param name="instanceId">The ID of the VM instance to get the state of.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains the <see cref="VmInstanceState"/> of the VM instance. </returns>
    Task<VmInstanceState> GetVmInstanceState(string instanceId);
}

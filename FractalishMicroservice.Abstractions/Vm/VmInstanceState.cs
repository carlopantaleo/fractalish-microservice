namespace FractalishMicroservice.Abstractions.Vm;

/// <summary>
/// Represents the state of a virtual machine (VM) instance.
/// </summary>
public enum VmInstanceState {
    /// <summary>
    /// The VM instance is being created.
    /// </summary>
    Pending,

    /// <summary>
    /// The VM instance is running.
    /// </summary>
    Running,

    /// <summary>
    /// The VM instance is being shut down.
    /// </summary>
    ShuttingDown,

    /// <summary>
    /// The VM instance has been terminated.
    /// </summary>
    Terminated,

    /// <summary>
    /// The VM instance is being stopped.
    /// </summary>
    Stopping,

    /// <summary>
    /// The VM instance has been stopped.
    /// </summary>
    Stopped
}

namespace FractalishMicroservice.Abstractions.Vm;

public enum VmInstanceState {
    Pending,
    Running,
    ShuttingDown,
    Terminated,
    Stopping,
    Stopped
}

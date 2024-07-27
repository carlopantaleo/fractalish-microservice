namespace FractalishMicroservice.Abstractions.Vm;

public interface IVmInstanceService
{
    Task<string> CreateVmInstance(string instanceType, string amiId);
    Task TerminateVmInstance(string instanceId);
    Task<VmInstanceState> GetVmInstanceState(string instanceId);
}

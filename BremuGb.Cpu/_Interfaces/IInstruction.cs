using BremuGb.Memory;

namespace BremuGb.Cpu
{
    public interface IInstruction
    {
        public void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory);
        public bool IsFetchNecessary();
    }
}

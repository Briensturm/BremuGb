using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class STOP : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.StopMode = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

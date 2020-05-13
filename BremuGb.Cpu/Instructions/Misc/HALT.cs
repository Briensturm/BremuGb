using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class HALT : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.HaltMode = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

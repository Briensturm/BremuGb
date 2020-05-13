using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class EI : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.ImeScheduled = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

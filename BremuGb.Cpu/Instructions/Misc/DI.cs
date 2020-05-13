using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class DI : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.InterruptMasterEnable = false;
            cpuState.ImeScheduled = false;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

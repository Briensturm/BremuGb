using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class JPHL : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.ProgramCounter = cpuState.Registers.HL;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

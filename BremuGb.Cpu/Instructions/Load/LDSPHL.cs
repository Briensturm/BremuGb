using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDSPHL : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 1:
                    cpuState.StackPointer = cpuState.Registers.HL;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

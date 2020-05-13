using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_HLP_A : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    mainMemory.WriteByte(cpuState.Registers.HL, cpuState.Registers.A);
                    break;
                case 1:
                    cpuState.Registers.HL++;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

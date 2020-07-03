using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDA_HLP_ : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    cpuState.Registers.A = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    cpuState.Registers.HL++;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDHA_C_ : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    cpuState.Registers.A = mainMemory.ReadByte((ushort)((0xFF << 8) | cpuState.Registers.C));
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

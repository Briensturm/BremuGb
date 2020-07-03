using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDH_C_A : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    mainMemory.WriteByte((ushort)((0xFF << 8) | cpuState.Registers.C), cpuState.Registers.A);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

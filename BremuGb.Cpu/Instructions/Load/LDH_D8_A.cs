using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDH_D8_A : InstructionBase
    {
        private byte _addressLsb;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _addressLsb = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 2:
                    mainMemory.WriteByte((ushort)((0xFF << 8) | _addressLsb), cpuState.Registers.A);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

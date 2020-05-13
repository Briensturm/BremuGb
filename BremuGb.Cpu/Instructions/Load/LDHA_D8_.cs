using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDHA_D8_ : InstructionBase
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
                    cpuState.Registers.A = mainMemory.ReadByte((ushort)((0xFF << 8) | _addressLsb));
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

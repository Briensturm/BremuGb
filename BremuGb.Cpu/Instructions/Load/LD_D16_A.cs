using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_D16_A : InstructionBase
    {
        private ushort _address;
        protected override int InstructionLength => 4;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 4:
                    _address = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 3:
                    _address |= (ushort)(mainMemory.ReadByte(cpuState.ProgramCounter++) << 8);
                    break;
                case 2:
                    mainMemory.WriteByte(_address, cpuState.Registers.A);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

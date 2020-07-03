using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDA_D16_ : InstructionBase
    {
        private ushort _address;
        private byte _writeData;
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
                    _writeData = mainMemory.ReadByte(_address);
                    break;
                case 1:
                    cpuState.Registers.A = _writeData;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

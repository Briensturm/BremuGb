using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_D16_SP : InstructionBase
    {
        private ushort _writeAddress;
        protected override int InstructionLength => 5;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 5:
                    _writeAddress = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 4:
                    _writeAddress |= (ushort)(mainMemory.ReadByte(cpuState.ProgramCounter++) << 8);                    
                    break;
                case 3:
                    //write lsb of stack pointer
                    mainMemory.WriteByte(_writeAddress, (byte)cpuState.StackPointer);
                    break;
                case 2:
                    //write msb of stack pointer
                    mainMemory.WriteByte((ushort)(_writeAddress + 1), (byte)(cpuState.StackPointer >> 8));
                    break;
                case 1:
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

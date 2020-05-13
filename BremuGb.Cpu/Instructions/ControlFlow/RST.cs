using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RST : InstructionBase
    {
        private ushort _jumpAddress;

        protected override int InstructionLength => 4;

        public RST(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 4:
                    //calculate jump address
                    _jumpAddress = (ushort)(_opcode & 0x38);
                    break;
                case 3:
                    //write msb of program counter to stack
                    mainMemory.WriteByte(--cpuState.StackPointer, (byte)(cpuState.ProgramCounter >> 8));                    
                    break;
                case 2:
                    //write lsb of program counter to stack
                    mainMemory.WriteByte(--cpuState.StackPointer, (byte)(cpuState.ProgramCounter & 0x00FF));
                    break;
                case 1:
                    //do the jump
                    cpuState.ProgramCounter = _jumpAddress;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

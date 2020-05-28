using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class CALLCC : InstructionBase
    {
        private ushort _jumpAddress;

        protected override int InstructionLength => 6;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 6:
                    //read jump address lsb
                    _jumpAddress = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 5:
                    //read jump address msb
                    _jumpAddress |= (ushort)(mainMemory.ReadByte(cpuState.ProgramCounter++) << 8);
                    break;
                case 4:
                    //set last cycle if condition is not met
                    if (!IsConditionMet(cpuState))
                        _remainingCycles = 1;
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

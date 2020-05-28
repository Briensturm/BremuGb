using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class JPCC : InstructionBase
    {
        private ushort _jumpAddress;

        protected override int InstructionLength => 4;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 4:
                    //read jump address lsb
                    _jumpAddress = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 3:
                    //read jump address msb
                    _jumpAddress |= (ushort)(mainMemory.ReadByte(cpuState.ProgramCounter++) << 8);
                    break;
                case 2:
                    //set last cycle if condition is not met
                    if (!IsConditionMet(cpuState))
                        _remainingCycles = 1;
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

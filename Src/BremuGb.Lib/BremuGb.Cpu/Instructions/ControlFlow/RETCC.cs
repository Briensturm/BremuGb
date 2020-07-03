using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RETCC : InstructionBase
    {
        private ushort _jumpAddress;

        protected override int InstructionLength => 5;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 5:
                    if (!IsConditionMet(cpuState))
                        _remainingCycles = 2;
                    break;
                case 4:
                    //read jump address lsb
                    _jumpAddress = mainMemory.ReadByte(cpuState.StackPointer++);
                    break;
                case 3:
                    //read jump address msb
                    _jumpAddress |= (ushort)(mainMemory.ReadByte(cpuState.StackPointer++) << 8);
                    break;
                case 2:
                    //do the jump
                    cpuState.ProgramCounter = _jumpAddress;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

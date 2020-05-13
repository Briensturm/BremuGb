using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class JRCC : InstructionBase
    {
        private sbyte _relativeAddress;

        protected override int InstructionLength => 3;

        public JRCC(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 3:
                    //read jump address lsb
                    _relativeAddress = (sbyte)mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 2:
                    //set last cycle if condition is not met
                    if (!IsConditionMet(cpuState))
                        _remainingCycles = 1;
                    break;
                case 1:
                    //do the jump
                    cpuState.ProgramCounter = (ushort)(cpuState.ProgramCounter + _relativeAddress);
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

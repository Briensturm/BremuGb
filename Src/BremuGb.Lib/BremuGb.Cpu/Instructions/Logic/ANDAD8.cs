using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ANDAD8 : InstructionBase
    {
        byte _data;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 2:
                    _data = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 1:
                    cpuState.Registers.A &= _data;
                    break;
            }            

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = true;
            cpuState.Registers.CarryFlag = false;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

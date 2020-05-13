using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class CPAD8 : InstructionBase
    {
        private byte _subData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _subData = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 1:  
                    cpuState.Registers.SubtractionFlag = true;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A - _subData == 0;
                    cpuState.Registers.HalfCarryFlag = _subData > (cpuState.Registers.A & 0xF);
                    cpuState.Registers.CarryFlag = _subData > cpuState.Registers.A;

                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

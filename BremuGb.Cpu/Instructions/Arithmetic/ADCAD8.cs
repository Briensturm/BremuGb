using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADCAD8 : InstructionBase
    {
        private byte _addData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _addData = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 1:
                    var oldValue = cpuState.Registers.A;

                    int result = cpuState.Registers.A + _addData;
                    if (cpuState.Registers.CarryFlag)
                        result++;

                    cpuState.Registers.A = (byte)result;

                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
                    cpuState.Registers.HalfCarryFlag = (((oldValue & 0xF) + (_addData & 0xF) 
                                                + (cpuState.Registers.CarryFlag ? 1 : 0)) & 0x10) == 0x10;
                    cpuState.Registers.CarryFlag = result > 0xFF;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

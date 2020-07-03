using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class INC_HL_ : InstructionBase
    {
        private byte _currentData;
        private byte _incrementedData;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    _incrementedData = (byte)(_currentData + 1);
                    mainMemory.WriteByte(cpuState.Registers.HL, _incrementedData);
                    break;
                case 1:
                    cpuState.Registers.ZeroFlag = _incrementedData == 0;
                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.HalfCarryFlag = (_incrementedData & 0x0F) == 0;
                    break;
            }
            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

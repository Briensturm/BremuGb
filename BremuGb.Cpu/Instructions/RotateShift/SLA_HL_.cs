using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SLA_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;
        int _hiBit;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    _hiBit = _currentData & 0x80;
                    _writeData = (byte)(_currentData << 1);

                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);

                    cpuState.Registers.CarryFlag = _hiBit == 0x80;
                    cpuState.Registers.ZeroFlag = _writeData == 0;
                    cpuState.Registers.HalfCarryFlag = false;
                    cpuState.Registers.SubtractionFlag = false;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

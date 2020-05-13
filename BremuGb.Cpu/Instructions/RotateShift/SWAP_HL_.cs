using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SWAP_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;

        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    byte lowNibble = (byte)(_currentData & 0x0F);
                    byte highNibble = (byte)(_currentData & 0xF0);

                    _writeData = (byte)((lowNibble << 4) | (highNibble >> 4));
                    break;
                case 1:
                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);

                    cpuState.Registers.ZeroFlag = _writeData == 0;
                    cpuState.Registers.HalfCarryFlag = false;
                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.CarryFlag = false;
                    break;
            }           

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

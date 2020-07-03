using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RL_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;
        int _bit;

        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    _bit = _currentData >> 7;

                    _writeData = (byte)(_currentData << 1);
                    if (cpuState.Registers.CarryFlag)
                        _writeData |= 0x01;

                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);

                    cpuState.Registers.CarryFlag = _bit == 1;
                    cpuState.Registers.ZeroFlag = _writeData == 0;
                    cpuState.Registers.HalfCarryFlag = false;
                    cpuState.Registers.SubtractionFlag = false;
                    break;
            }            

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

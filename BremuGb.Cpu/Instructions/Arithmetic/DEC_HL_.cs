using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class DEC_HL_ : InstructionBase
    {
        private byte _currentData;
        private byte _decrementedData;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    _decrementedData = (byte)(_currentData - 1);
                    mainMemory.WriteByte(cpuState.Registers.HL, _decrementedData);
                    break;
                case 1:
                    cpuState.Registers.ZeroFlag = _decrementedData == 0;
                    cpuState.Registers.SubtractionFlag = true;
                    cpuState.Registers.HalfCarryFlag = (_currentData & 0x0F) == 0;
                    break;
            }
            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

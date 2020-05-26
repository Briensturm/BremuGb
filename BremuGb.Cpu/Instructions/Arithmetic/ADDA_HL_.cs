using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADDA_HL_ : InstructionBase
    {
        private byte _addData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 2:
                    _addData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    var oldValue = cpuState.Registers.A;

                    cpuState.Registers.A += _addData;

                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
                    cpuState.Registers.HalfCarryFlag = (((oldValue & 0xF) + (_addData & 0xF)) & 0x10) == 0x10;
                    cpuState.Registers.CarryFlag = cpuState.Registers.A < oldValue;

                    break;
            }           

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

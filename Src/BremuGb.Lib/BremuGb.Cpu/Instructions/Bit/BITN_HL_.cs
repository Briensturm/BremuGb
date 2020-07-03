using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class BITN_HL_ : InstructionBase
    {
        byte _currentData;

        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {          
            switch (_remainingCycles)
            {
                case 2:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    var bitIndex = (_opcode & 0x38) >> 3;
                    cpuState.Registers.ZeroFlag = ((byte)(_currentData >> bitIndex) & 0x01) == 0;

                    cpuState.Registers.HalfCarryFlag = true;
                    cpuState.Registers.SubtractionFlag = false;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

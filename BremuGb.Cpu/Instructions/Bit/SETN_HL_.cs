using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SETN_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;
        protected override int InstructionLength => 3;
        public SETN_HL_(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {          
            switch (_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    var bitIndex = (_opcode & 0x38) >> 3;
                    _writeData = (byte)(_currentData | (0x01 << bitIndex));
                    break;
                case 1:
                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

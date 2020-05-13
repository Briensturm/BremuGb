using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDR8_HL_ : InstructionBase
    {
        private byte _loadData;
        private int _registerIndex;
        protected override int InstructionLength => 2;
        public LDR8_HL_(byte opcode) : base(opcode)
        {
            _registerIndex = (opcode >> 3) & 0x07;
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _loadData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    cpuState.Registers[_registerIndex] = _loadData;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

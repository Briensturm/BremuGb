using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDA_DE_ : InstructionBase
    {
        private byte _loadData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _loadData = mainMemory.ReadByte(cpuState.Registers.DE);
                    break;
                case 1:
                    cpuState.Registers.A = _loadData;
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

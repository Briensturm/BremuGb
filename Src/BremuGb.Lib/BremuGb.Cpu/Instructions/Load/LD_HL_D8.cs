using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_HL_D8 : InstructionBase
    {
        private byte _loadData;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _loadData = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 2:
                    mainMemory.WriteByte(cpuState.Registers.HL, _loadData);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

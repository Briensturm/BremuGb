using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SCF : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.Registers.CarryFlag = true;
            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

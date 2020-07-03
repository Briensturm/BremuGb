using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class CPL : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.Registers.A = (byte)~cpuState.Registers.A;

            cpuState.Registers.SubtractionFlag = true;
            cpuState.Registers.HalfCarryFlag = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

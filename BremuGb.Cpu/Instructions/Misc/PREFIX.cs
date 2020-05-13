using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class PREFIX : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            //set prefix flag in cpu state
            cpuState.InstructionPrefix = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

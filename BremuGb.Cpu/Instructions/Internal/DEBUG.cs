#if DEBUG
using System;

using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class DEBUG : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            Console.WriteLine("DEBUG instruction executed");

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}
#endif

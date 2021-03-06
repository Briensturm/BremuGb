﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class NOP : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

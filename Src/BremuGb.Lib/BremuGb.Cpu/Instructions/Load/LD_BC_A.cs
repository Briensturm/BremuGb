﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_BC_A : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    mainMemory.WriteByte(cpuState.Registers.BC, cpuState.Registers.A);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

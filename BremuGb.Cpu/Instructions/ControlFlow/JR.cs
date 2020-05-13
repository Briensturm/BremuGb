﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class JR : InstructionBase
    {
        private sbyte _relativeAddress;

        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 3:
                    //read jump address lsb
                    _relativeAddress = (sbyte)mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 1:
                    //do the jump
                    cpuState.ProgramCounter = (ushort)(cpuState.ProgramCounter + _relativeAddress);
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

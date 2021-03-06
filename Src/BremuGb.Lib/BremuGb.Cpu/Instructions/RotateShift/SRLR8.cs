﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SRLR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;

            var loBit = cpuState.Registers[registerIndex] & 0x01;
            cpuState.Registers.CarryFlag = loBit == 0x01;
            
            cpuState.Registers[registerIndex] = (ushort)(cpuState.Registers[registerIndex] >> 1);

            cpuState.Registers.ZeroFlag = cpuState.Registers[registerIndex] == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

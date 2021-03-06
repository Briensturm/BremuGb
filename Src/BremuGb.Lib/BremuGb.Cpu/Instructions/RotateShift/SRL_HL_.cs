﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SRL_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;
        int _loBit;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _currentData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 2:
                    _loBit = _currentData & 0x01;           
                    _writeData = (byte)(_currentData >> 1);

                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);

                    cpuState.Registers.ZeroFlag = _writeData == 0;
                    cpuState.Registers.HalfCarryFlag = false;
                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.CarryFlag = _loBit == 0x01;
                    break;
            }            

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

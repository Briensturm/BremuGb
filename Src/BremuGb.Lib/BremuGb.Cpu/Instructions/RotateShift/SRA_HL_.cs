﻿using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SRA_HL_ : InstructionBase
    {
        byte _currentData;
        byte _writeData;
        int _hiBit;
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
                    _hiBit = _currentData & 0x80;
                    _loBit = _currentData & 0x01;                    

                    _writeData = (byte)(_currentData >> 1);
                    _writeData |= (byte)_hiBit;

                    mainMemory.WriteByte(cpuState.Registers.HL, _writeData);

                    cpuState.Registers.CarryFlag = _loBit == 0x01;
                    cpuState.Registers.ZeroFlag = _writeData == 0;
                    cpuState.Registers.HalfCarryFlag = false;
                    cpuState.Registers.SubtractionFlag = false;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

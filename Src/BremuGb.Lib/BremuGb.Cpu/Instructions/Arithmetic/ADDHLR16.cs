using BremuGb.Memory;
using System;

namespace BremuGb.Cpu.Instructions
{
    public class ADDHLR16 : InstructionBase
    {
        private ushort _oldValue;
        private ushort _addData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    switch(_opcode)
                    {
                        case 0x09:
                            _addData = cpuState.Registers.BC;
                            break;
                        case 0x19:
                            _addData = cpuState.Registers.DE;
                            break;
                        case 0x29:
                            _addData = cpuState.Registers.HL;
                            break;
                        case 0x39:
                            _addData = cpuState.StackPointer;
                            break;
                        default:
                            throw new InvalidOperationException($"0x{_opcode:X2} is not a valid opcode for instruction {GetType()}");
                            
                    }

                    _oldValue = cpuState.Registers.HL;
                    break;
                case 1:
                    cpuState.Registers.HL += _addData;

                    cpuState.Registers.SubtractionFlag = false;                    
                    cpuState.Registers.CarryFlag = _addData > (0xFFFF - _oldValue);
                    cpuState.Registers.HalfCarryFlag = (((_addData & 0xFFF) + (_oldValue & 0xFFF)) & 0x1000) == 0x1000;
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

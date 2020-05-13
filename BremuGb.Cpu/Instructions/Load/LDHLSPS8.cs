using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDHLSPS8 : InstructionBase
    {
        private sbyte _signedValue;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _signedValue = (sbyte)mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 2:
                    cpuState.Registers.HL = (ushort)(cpuState.StackPointer + _signedValue);
                    break;
                case 1:
                    if (_signedValue > 0)
                    {
                        cpuState.Registers.CarryFlag = (0xFFFF - cpuState.StackPointer) < _signedValue;
                        cpuState.Registers.HalfCarryFlag = (0xFF - (cpuState.StackPointer & 0x00FF)) < _signedValue;
                    }
                    else if (_signedValue < 0)
                    {
                        cpuState.Registers.CarryFlag = cpuState.StackPointer < _signedValue * (-1);
                        cpuState.Registers.HalfCarryFlag = (cpuState.StackPointer & 0x00FF) < _signedValue;
                    }
                    else
                    {
                        cpuState.Registers.CarryFlag = false;
                        cpuState.Registers.HalfCarryFlag = false;
                    }                    

                    cpuState.Registers.ZeroFlag = false;
                    cpuState.Registers.SubtractionFlag = false;                    
                    break;                
            }            

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

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
                    if (_signedValue >= 0)
                    {
                        cpuState.Registers.CarryFlag = ((cpuState.StackPointer & 0xFF) + (_signedValue)) > 0xFF;
                        cpuState.Registers.HalfCarryFlag = ((cpuState.StackPointer & 0xF) + (_signedValue & 0xF)) > 0xF;
                    }
                    else
                    {
                        cpuState.Registers.CarryFlag = ((cpuState.StackPointer + _signedValue) & 0xFF) <= (cpuState.StackPointer & 0xFF);
                        cpuState.Registers.HalfCarryFlag = ((cpuState.StackPointer + _signedValue) & 0xF) <= (cpuState.StackPointer & 0xF);
                    }

                    cpuState.Registers.ZeroFlag = false;
                    cpuState.Registers.SubtractionFlag = false;                    
                    break;                
            }            

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

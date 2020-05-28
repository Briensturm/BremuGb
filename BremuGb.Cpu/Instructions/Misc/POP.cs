using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class POP : InstructionBase
    {
        private byte _registerBits;

        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        { 
            switch(_remainingCycles)
            {
                case 3:
                    _registerBits = (byte)((_opcode >> 4) & 0x03);

                    var lsbData = mainMemory.ReadByte(cpuState.StackPointer++);
                    WriteToRegister(cpuState, lsbData, true);                    
                    break;
                case 2:
                    var msbData = mainMemory.ReadByte(cpuState.StackPointer++);
                    WriteToRegister(cpuState, msbData, false);
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }

        private void WriteToRegister(ICpuState cpuState, byte data, bool isLsb)
        {
            switch(_registerBits)
            {
                case 0b00:
                    if (isLsb)
                        cpuState.Registers.C = data;
                    else
                        cpuState.Registers.B = data;
                    break;
                case 0b01:
                    if (isLsb)
                        cpuState.Registers.E = data;
                    else
                        cpuState.Registers.D = data;
                    break;
                case 0b10:
                    if (isLsb)
                        cpuState.Registers.L = data;
                    else
                        cpuState.Registers.H = data;
                    break;
                case 0b11:
                    if (isLsb)
                        cpuState.Registers.F = data;
                    else
                        cpuState.Registers.A = data;
                    break;
            }
        }
    }
}

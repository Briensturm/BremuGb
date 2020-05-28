using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class PUSH : InstructionBase
    {
        private byte _registerBits;

        private byte _msbData;
        private byte _lsbData;

        protected override int InstructionLength => 4;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 4:                        
                    _registerBits = (byte)((_opcode >> 4) & 0x03);                        
                    LoadWriteData(cpuState);                    
                    break;
                case 3:
                    mainMemory.WriteByte(--cpuState.StackPointer, _msbData);
                    break;
                case 2:
                    mainMemory.WriteByte(--cpuState.StackPointer, _lsbData);
                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }

        private void LoadWriteData(ICpuState cpuState)
        {
            switch(_registerBits)
            {
                case 0b00:
                    _msbData = cpuState.Registers.B;
                    _lsbData = cpuState.Registers.C;
                    break;
                case 0b01:
                    _msbData = cpuState.Registers.D;
                    _lsbData = cpuState.Registers.E;
                    break;
                case 0b10:
                    _msbData = cpuState.Registers.H;
                    _lsbData = cpuState.Registers.L;
                    break;
                case 0b11:
                    _msbData = cpuState.Registers.A;
                    _lsbData = cpuState.Registers.F;
                    break;
            }
        }
    }
}

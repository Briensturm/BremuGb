using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class INCR16 : InstructionBase
    {
        private byte _registerBits;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 2:
                    _registerBits = (byte)((_opcode >> 4) & 0x03);
                    break;
                case 1:
                    switch(_registerBits)
                    {
                        case 0b00:
                            cpuState.Registers.BC++;
                            break;
                        case 0b01:
                            cpuState.Registers.DE++;
                            break;
                        case 0b10:
                            cpuState.Registers.HL++;
                            break;
                        case 0b11:
                            cpuState.StackPointer++;
                            break;
                    }
                    break;
            }
            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

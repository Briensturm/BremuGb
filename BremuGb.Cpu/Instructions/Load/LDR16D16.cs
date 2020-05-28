using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LDR16D16 : InstructionBase
    {
        private byte _registerBits;
        private ushort _loadData;
        protected override int InstructionLength => 3;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 3:
                    _registerBits = (byte)((_opcode >> 4) & 0x03);

                    _loadData = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 2:
                    _loadData |= (ushort)(mainMemory.ReadByte(cpuState.ProgramCounter++) << 8);
                    break;
                case 1:
                    switch (_registerBits)
                    {
                        case 0b00:
                            cpuState.Registers.BC = _loadData;
                            break;
                        case 0b01:
                            cpuState.Registers.DE = _loadData;
                            break;
                        case 0b10:
                            cpuState.Registers.HL = _loadData;
                            break;
                        case 0b11:
                            cpuState.StackPointer = _loadData;
                            break;
                    }
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

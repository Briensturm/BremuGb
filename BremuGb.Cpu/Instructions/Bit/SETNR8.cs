using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SETNR8 : InstructionBase
    {
        protected override int InstructionLength => 1;
        public SETNR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            //decode register and bit
            var bitIndex = (_opcode & 0x38) >> 3;
            var registerIndex = _opcode & 0x07;

            cpuState.Registers[registerIndex] = (ushort)(cpuState.Registers[registerIndex] | (0x01 << bitIndex));

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

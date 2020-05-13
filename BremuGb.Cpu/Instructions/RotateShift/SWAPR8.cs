using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SWAPR8 : InstructionBase
    {
        protected override int InstructionLength => 1;
        public SWAPR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.CarryFlag = false;

            byte lowNibble = (byte)(cpuState.Registers[registerIndex] & 0x0F);
            byte highNibble = (byte)(cpuState.Registers[registerIndex] & 0xF0);

            cpuState.Registers[registerIndex] = (ushort)((lowNibble << 4) | (highNibble >> 4));            

            cpuState.Registers.ZeroFlag = cpuState.Registers[registerIndex] == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

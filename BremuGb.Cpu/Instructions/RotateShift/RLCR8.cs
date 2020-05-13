using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RLCR8 : InstructionBase
    {
        protected override int InstructionLength => 1;
        public RLCR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;

            var bit = cpuState.Registers[_opcode] >> 7;
            cpuState.Registers.CarryFlag = bit == 1;

            cpuState.Registers[_opcode] = (ushort)((cpuState.Registers[_opcode] << 1) | bit);
            cpuState.Registers.ZeroFlag = cpuState.Registers[_opcode] == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

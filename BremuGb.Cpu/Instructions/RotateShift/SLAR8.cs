using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SLAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;
        public SLAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;

            var hiBit = cpuState.Registers[registerIndex] & 0x80;
            cpuState.Registers.CarryFlag = hiBit == 0x80;
            
            cpuState.Registers[registerIndex] = (ushort)(cpuState.Registers[registerIndex] << 1);

            cpuState.Registers.ZeroFlag = cpuState.Registers[registerIndex] == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

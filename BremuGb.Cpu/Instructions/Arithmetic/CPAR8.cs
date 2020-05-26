using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class CPAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public CPAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.SubtractionFlag = true;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A - (byte)cpuState.Registers[registerIndex] == 0;
            cpuState.Registers.HalfCarryFlag = ((cpuState.Registers.A & 0xF) - (cpuState.Registers[registerIndex] & 0xF)) < 0;
            cpuState.Registers.CarryFlag = (byte)cpuState.Registers[registerIndex] > cpuState.Registers.A;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

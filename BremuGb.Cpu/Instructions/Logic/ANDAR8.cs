using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ANDAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public ANDAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.A &= (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = true;
            cpuState.Registers.CarryFlag = false;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

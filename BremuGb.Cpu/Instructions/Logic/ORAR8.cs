using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ORAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.A |= (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.CarryFlag = false;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

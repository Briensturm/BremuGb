using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class INCR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode >> 3;
            cpuState.Registers[registerIndex]++;

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers[registerIndex] == 0;
            cpuState.Registers.HalfCarryFlag = (cpuState.Registers[registerIndex] & 0x0F) == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

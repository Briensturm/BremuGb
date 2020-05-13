using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RLA : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = false;

            var bit = cpuState.Registers.A >> 7;            
            
            cpuState.Registers.A = (byte)(cpuState.Registers.A << 1);
            if (cpuState.Registers.CarryFlag)
                cpuState.Registers.A |= 0x01;

            cpuState.Registers.CarryFlag = bit == 1;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

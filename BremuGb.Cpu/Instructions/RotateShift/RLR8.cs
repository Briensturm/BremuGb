using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class RLR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.SubtractionFlag = false;

            var bit = cpuState.Registers[registerIndex] >> 7;            
            
            cpuState.Registers[registerIndex] = (ushort)(cpuState.Registers[registerIndex] << 1);
            if (cpuState.Registers.CarryFlag)
                cpuState.Registers[registerIndex] |= 0x01;

            cpuState.Registers.ZeroFlag = cpuState.Registers[registerIndex] == 0;
            cpuState.Registers.CarryFlag = bit == 1;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

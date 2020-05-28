using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class BITNR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            cpuState.Registers.HalfCarryFlag = true;
            cpuState.Registers.SubtractionFlag = false;

            //decode register and bit
            var bitIndex = (_opcode & 0x38) >> 3;
            var registerIndex = _opcode & 0x07;

            cpuState.Registers.ZeroFlag = ((byte)(cpuState.Registers[registerIndex] >> bitIndex) & 0x01) == 0;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

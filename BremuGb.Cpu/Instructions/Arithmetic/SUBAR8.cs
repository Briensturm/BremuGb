using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SUBAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public SUBAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;
            var oldValue = cpuState.Registers.A;
            byte subData = (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.A -= subData;

            cpuState.Registers.SubtractionFlag = true;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = ((oldValue & 0xF) - (subData & 0xF)) < 0;
            cpuState.Registers.CarryFlag = subData > oldValue;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

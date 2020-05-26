using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADDAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public ADDAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;
            var oldValue = cpuState.Registers.A;
            byte addData = (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.A += addData;

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = (((oldValue & 0xF) + (addData & 0xF)) & 0x10) == 0x10;
            cpuState.Registers.CarryFlag = cpuState.Registers.A < oldValue;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

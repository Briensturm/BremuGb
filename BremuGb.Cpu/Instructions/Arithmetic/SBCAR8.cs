using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SBCAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public SBCAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;
            var oldValue = cpuState.Registers.A;
            byte subData = (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.A -= subData;

            if (cpuState.Registers.CarryFlag)
                cpuState.Registers.A--;

            cpuState.Registers.SubtractionFlag = true;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = ((subData & 0x0F)
                                            + (cpuState.Registers.CarryFlag ? 1 : 0)) > (oldValue & 0xF);
            cpuState.Registers.CarryFlag = (subData + (cpuState.Registers.CarryFlag ? 1 : 0)) > oldValue;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

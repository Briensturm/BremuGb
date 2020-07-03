using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADCAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;
            var oldValue = cpuState.Registers.A;
            byte addData = (byte)cpuState.Registers[registerIndex];

            int result = cpuState.Registers.A + addData;
            if (cpuState.Registers.CarryFlag)
                result++;

            cpuState.Registers.A = (byte)result;

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = (((oldValue & 0xF) + (addData & 0xF)
                                                + (cpuState.Registers.CarryFlag ? 1 : 0)) & 0x10) == 0x10;
            cpuState.Registers.CarryFlag = result > 0xFF;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

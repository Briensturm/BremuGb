using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADCAR8 : InstructionBase
    {
        protected override int InstructionLength => 1;

        public ADCAR8(byte opcode) : base(opcode)
        {
        }

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var registerIndex = _opcode & 0x07;
            var oldValue = cpuState.Registers.A;
            byte addData = (byte)cpuState.Registers[registerIndex];

            cpuState.Registers.A += addData;

            if (cpuState.Registers.CarryFlag)
                cpuState.Registers.A++;

            cpuState.Registers.SubtractionFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            cpuState.Registers.HalfCarryFlag = addData + (cpuState.Registers.CarryFlag ? 1 : 0) > (0xF - (oldValue & 0xF));
            
            cpuState.Registers.CarryFlag = cpuState.Registers.A < oldValue;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class LD_HL_R8 : InstructionBase
    {
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    var registerIndex = _opcode & 0x07;
                    mainMemory.WriteByte(cpuState.Registers.HL, (byte)cpuState.Registers[registerIndex]);
                    break;
            }                    

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

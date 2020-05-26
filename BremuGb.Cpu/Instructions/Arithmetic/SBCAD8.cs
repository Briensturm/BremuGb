using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SBCAD8 : InstructionBase
    {
        private byte _subData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _subData = mainMemory.ReadByte(cpuState.ProgramCounter++);
                    break;
                case 1:
                    var oldValue = cpuState.Registers.A;

                    cpuState.Registers.A -= _subData;

                    if (cpuState.Registers.CarryFlag)
                        cpuState.Registers.A--;

                    cpuState.Registers.SubtractionFlag = true;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
                    cpuState.Registers.HalfCarryFlag = ((_subData & 0x0F) 
                                            + (cpuState.Registers.CarryFlag ? 1 : 0)) > (oldValue & 0xF);
                    cpuState.Registers.CarryFlag = (_subData + (cpuState.Registers.CarryFlag ? 1 : 0)) > oldValue;

                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class ADCA_HL_ : InstructionBase
    {
        private byte _addData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch (_remainingCycles)
            {
                case 2:
                    _addData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    var oldValue = cpuState.Registers.A;

                    cpuState.Registers.A += _addData;

                    if (cpuState.Registers.CarryFlag)
                        cpuState.Registers.A++;

                    cpuState.Registers.SubtractionFlag = false;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
                    cpuState.Registers.HalfCarryFlag = _addData + (cpuState.Registers.CarryFlag ? 1 : 0) > (0xF - (oldValue & 0xF));
                    
                    cpuState.Registers.CarryFlag = cpuState.Registers.A < oldValue;

                    break;
            }

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

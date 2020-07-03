using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class SUBA_HL_ : InstructionBase
    {
        private byte _subData;
        protected override int InstructionLength => 2;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            switch(_remainingCycles)
            {
                case 2:
                    _subData = mainMemory.ReadByte(cpuState.Registers.HL);
                    break;
                case 1:
                    var oldValue = cpuState.Registers.A;

                    cpuState.Registers.A -= _subData;

                    cpuState.Registers.SubtractionFlag = true;
                    cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
                    cpuState.Registers.HalfCarryFlag = ((oldValue & 0xF) - (_subData & 0xF)) < 0;
                    cpuState.Registers.CarryFlag = _subData > oldValue;

                    break;
            }           

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

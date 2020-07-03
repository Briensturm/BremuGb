using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Cpu.Instructions
{
    public class HALT : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            var interruptEnable = mainMemory.ReadByte(MiscRegisters.InterruptEnable);
            var interruptFlags = mainMemory.ReadByte(MiscRegisters.InterruptFlags);

            if (!cpuState.InterruptMasterEnable && (interruptEnable & interruptFlags & 0x1F) != 0)
                cpuState.HaltBug = true;    
            else
                cpuState.HaltMode = true;

            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

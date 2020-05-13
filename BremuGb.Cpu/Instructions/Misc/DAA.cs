using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public class DAA : InstructionBase
    {
        protected override int InstructionLength => 1;

        public override void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            if (!cpuState.Registers.SubtractionFlag)
            {  
                if (cpuState.Registers.CarryFlag || cpuState.Registers.A > 0x99) 
                { 
                    cpuState.Registers.A += 0x60; 
                    cpuState.Registers.CarryFlag = true; 
                }

                if (cpuState.Registers.HalfCarryFlag || (cpuState.Registers.A & 0x0f) > 0x09) 
                    cpuState.Registers.A += 0x6; 
            }
            else
            { 
                if (cpuState.Registers.CarryFlag) 
                    cpuState.Registers.A -= 0x60; 

                if (cpuState.Registers.HalfCarryFlag)
                    cpuState.Registers.A -= 0x6; 
            }

            cpuState.Registers.HalfCarryFlag = false;
            cpuState.Registers.ZeroFlag = cpuState.Registers.A == 0;
            base.ExecuteCycle(cpuState, mainMemory);
        }
    }
}

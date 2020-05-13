namespace BremuGb.Cpu
{
    public class CpuState : ICpuState
    {
        public void Reset()
        {
            ProgramCounter = 0x0100;
            StackPointer = 0xFFFE;
            InterruptMasterEnable = false;
            InstructionPrefix = false;
            HaltMode = false;
            StopMode = false;

            ImeScheduled = false;            

            Registers.Reset();
        }

        public ushort ProgramCounter { get; set; }
        public ushort StackPointer { get; set; }
        public bool InterruptMasterEnable { get; set; }

        public CpuRegisters Registers { get; } = new CpuRegisters();
        public bool InstructionPrefix { get; set; }
        public bool HaltMode { get; set; }
        public bool StopMode { get; set; }
        public bool ImeScheduled { get; set; }
    }
}

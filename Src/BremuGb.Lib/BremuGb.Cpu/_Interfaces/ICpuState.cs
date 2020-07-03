namespace BremuGb.Cpu
{
    public interface ICpuState
    {
        public void Reset();

        public ushort ProgramCounter { get; set; }
        public ushort StackPointer { get; set; }

        public bool InterruptMasterEnable { get; set; }
        public bool ImeScheduled { get; set; }
        public bool HaltMode { get; set; }
        public bool HaltBug { get; set; }
        public bool StopMode { get; set; }
        public bool InstructionPrefix { get; set; }

        public CpuRegisters Registers { get; }        
    }
}

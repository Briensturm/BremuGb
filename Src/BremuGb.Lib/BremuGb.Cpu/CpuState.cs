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

        public string LogState()
        {
            return $"SP: 0x{StackPointer:X4} PC: 0x{ProgramCounter:X4} A: 0x{Registers.A:X2} B: 0x{Registers.B:X2} C: 0x{Registers.C:X2} D: 0x{Registers.D:X2} E: 0x{Registers.E:X2} H: 0x{Registers.H:X2} L: 0x{Registers.L:X2}";
        }

        public ushort ProgramCounter { get; set; }
        public ushort StackPointer { get; set; }
        public bool InterruptMasterEnable { get; set; }

        public CpuRegisters Registers { get; } = new CpuRegisters();
        public bool InstructionPrefix { get; set; }
        public bool HaltMode { get; set; }
        public bool HaltBug { get; set; }
        public bool StopMode { get; set; }
        public bool ImeScheduled { get; set; }
    }
}

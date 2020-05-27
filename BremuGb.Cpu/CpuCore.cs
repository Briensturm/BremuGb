using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Cpu.Instructions;
using BremuGb.Common;

namespace BremuGb.Cpu
{
    public class CpuCore : ICpuCore
    {
        private readonly ICpuState _cpuState;

        private readonly IRandomAccessMemory _mainMemory;
        private IInstruction _currentInstruction;

        private readonly Logger _logger;

        private bool IsCpuRunning => !_cpuState.StopMode && !_cpuState.HaltMode;

        private IInstruction CurrentInstruction
        {
            get
            {
                {
                    if (_currentInstruction == null)
                        _currentInstruction = GetNextInstruction();

                    return _currentInstruction;
                }
            }

            set
            {
                _currentInstruction = value;
            }
        }

        public CpuCore(IRandomAccessMemory mainMemory, ICpuState cpuState, Logger logger)
        {
            _mainMemory = mainMemory;
            _cpuState = cpuState;

            _logger = logger;

            Reset();
        }

        public void AdvanceMachineCycle()
        {        
            //execute one cycle of the instruction, if not in halt/stop
            if(IsCpuRunning)
                CurrentInstruction.ExecuteCycle(_cpuState, _mainMemory);            

            if (CurrentInstruction.IsFetchNecessary())
            {
                //check for interrupts
                var readyInterrupts = GetRequestedAndEnabledInterrupts();
                if(readyInterrupts != 0 && !_cpuState.InstructionPrefix)
                {
                    _cpuState.HaltMode = false;
                    _cpuState.StopMode = false;

                    if (_cpuState.InterruptMasterEnable)
                        CurrentInstruction = new LDISR(readyInterrupts);
                    else
                        CurrentInstruction = GetNextInstruction();
                }

                else if (IsCpuRunning)
                    CurrentInstruction = GetNextInstruction();
            }                

            //delayed EI handling
            if(_cpuState.ImeScheduled)
            {
                _cpuState.ImeScheduled = false;
                _cpuState.InterruptMasterEnable = true;
            }
        }

        public void Reset()
        {
            _cpuState.Reset();

            CurrentInstruction = null;

            //set initial memory registers?
           
        }

        private IInstruction GetNextInstruction()
        {
            var nextOpcode = _mainMemory.ReadByte(_cpuState.ProgramCounter++);

            if (_cpuState.InstructionPrefix)
            {
                _cpuState.InstructionPrefix = false;
                var nextPrefixedInstruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(nextOpcode);

                _logger.Log($"{nextPrefixedInstruction.GetType().Name} 0x{nextOpcode:X2} {((CpuState)_cpuState).LogState()}");

                return nextPrefixedInstruction;
            }

            var nextInstruction = InstructionDecoder.GetInstructionFromOpcode(nextOpcode);

            _logger.Log($"{ ((CpuState)_cpuState).LogState()}");
            _logger.Log($"{nextInstruction.GetType().Name} 0x{nextOpcode:X2}");

            if (_cpuState.HaltBug)
            {
                _cpuState.ProgramCounter--;
                _cpuState.HaltBug = false;
            }

            return nextInstruction;     
        }

        private byte GetRequestedAndEnabledInterrupts()
        {
            //Console.WriteLine("read from cpu core");
            var interruptEnable = _mainMemory.ReadByte(MiscRegisters.InterruptEnable);
            var interruptFlags = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);

            return (byte)(interruptEnable & interruptFlags & 0x1F);
        }
    }
}

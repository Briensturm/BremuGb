using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Common;

namespace BremuGb.Cpu
{
    public class CpuCore : ICpuCore
    {
        private readonly ICpuState _cpuState;

        private readonly IRandomAccessMemory _mainMemory;
        private IInstruction _currentInstruction;

        InstructionDecoder _instructionDecoder;

        private readonly Logger _logger;

        private bool IsCpuRunning => !_cpuState.StopMode && !_cpuState.HaltMode;

        private IInstruction CurrentInstruction
        {
            get
            {
                if (_currentInstruction == null)
                    _currentInstruction = GetNextInstruction();

                return _currentInstruction;
            }

            set => _currentInstruction = value;
        }

        public CpuCore(IRandomAccessMemory mainMemory, ICpuState cpuState, Logger logger)
        {
            _mainMemory = mainMemory;
            _cpuState = cpuState;

            _instructionDecoder = new InstructionDecoder();

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
                        CurrentInstruction = _instructionDecoder.GetInterruptServiceRoutineInstruction(readyInterrupts);
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
        }

        private IInstruction GetNextInstruction()
        {
            var nextOpcode = _mainMemory.ReadByte(_cpuState.ProgramCounter++);            

            if (_cpuState.InstructionPrefix)
            {
                _cpuState.InstructionPrefix = false;
                return _instructionDecoder.DecodeInstruction(nextOpcode, true);
            }

            if (_cpuState.HaltBug)
            {
                _cpuState.ProgramCounter--;
                _cpuState.HaltBug = false;
            }

            return _instructionDecoder.DecodeInstruction(nextOpcode);    
        }

        private byte GetRequestedAndEnabledInterrupts()
        {
            var interruptFlags = _mainMemory.ReadByte(MiscRegisters.InterruptFlags) & 0x1F;
            if (interruptFlags == 0)
                return 0;

            var interruptEnable = _mainMemory.ReadByte(MiscRegisters.InterruptEnable);            

            return (byte)(interruptEnable & interruptFlags);
        }
    }
}

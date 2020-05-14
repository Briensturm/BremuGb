using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu
{
    public class CpuCore : ICpuCore
    {
        private ICpuState _cpuState;

        private IRandomAccessMemory _mainMemory;
        private IInstruction _currentInstruction;

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

        public CpuCore(IRandomAccessMemory mainMemory, ICpuState cpuState)
        {
            _mainMemory = mainMemory;
            _cpuState = cpuState;

            Reset();
        }

        public void ExecuteCpuCycle()
        {        
            //execute one cycle of the instructions, if not in halt/stop
            if(IsCpuRunning)
                CurrentInstruction.ExecuteCycle(_cpuState, _mainMemory);

            if (CurrentInstruction.IsFetchNecessary())
            {
                //check for interrupts
                var interruptInstruction = TryGetInterruptInstruction();

                if (interruptInstruction != null)
                {
                    CurrentInstruction = interruptInstruction;

                    //interrupts disable halt/stop, if any
                    _cpuState.HaltMode = false;
                    _cpuState.StopMode = false;
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
                return InstructionDecoder.GetPrefixedInstructionFromOpcode(nextOpcode);                    
            }            

            return InstructionDecoder.GetInstructionFromOpcode(nextOpcode);     
        }

        //returns corresponding LDISR instruction if enabled interrupt occured, null otherwise
        private IInstruction TryGetInterruptInstruction()
        {
            var interruptEnable = _mainMemory.ReadByte(MiscRegisters.c_InterruptEnable);
            var interruptFlags = _mainMemory.ReadByte(MiscRegisters.c_InterruptEnable);

            var readyInterrupts = (byte)(interruptEnable & interruptFlags);

            if (_cpuState.InterruptMasterEnable && readyInterrupts != 0)
                return new LDISR(readyInterrupts);

            return null;
        }
    }
}

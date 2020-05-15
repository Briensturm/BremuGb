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
            //execute one cycle of the instruction, if not in halt/stop
            if(IsCpuRunning)
                CurrentInstruction.ExecuteCycle(_cpuState, _mainMemory);

            if (_cpuState.HaltBug)
            {
                CurrentInstruction = GetNextInstruction();
                _cpuState.ProgramCounter--;
                return;
            }

            if (CurrentInstruction.IsFetchNecessary())
            {
                //check for interrupts
                var readyInterrupts = GetRequestedAndEnabledInterrupts();
                if(readyInterrupts != 0)
                {
                    //interrupts disable halt/stop (delayed by one cycle)
                    if (_cpuState.HaltMode || _cpuState.StopMode)
                    {
                        CurrentInstruction = new NOP();
                        _cpuState.HaltMode = false;
                        _cpuState.StopMode = false;
                    }
                    else if (_cpuState.InterruptMasterEnable)
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
                return InstructionDecoder.GetPrefixedInstructionFromOpcode(nextOpcode);                    
            }            

            return InstructionDecoder.GetInstructionFromOpcode(nextOpcode);     
        }

        private byte GetRequestedAndEnabledInterrupts()
        {
            var interruptEnable = _mainMemory.ReadByte(MiscRegisters.InterruptEnable);
            var interruptFlags = _mainMemory.ReadByte(MiscRegisters.InterruptEnable);

            return (byte)(interruptEnable & interruptFlags & 0x1F);
        }
    }
}

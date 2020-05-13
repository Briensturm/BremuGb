using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Cpu
{
    public class CpuCore : ICpuCore
    {
        private ICpuState _cpuState;

        private IRandomAccessMemory _mainMemory;
        private IInstruction _currentInstruction;

        public CpuCore(IRandomAccessMemory mainMemory, ICpuState cpuState)
        {
            _mainMemory = mainMemory;
            _cpuState = cpuState;

            Reset();
        }

        public void ExecuteCpuCycle()
        {
            //TODO: Implement HALT/STOP handling

            if (_currentInstruction == null)
                LoadNextInstruction();

            _currentInstruction.ExecuteCycle(_cpuState, _mainMemory);

            //check if fetch
            if (_currentInstruction.IsFetchNecessary())
            {
                //check for interrupts
                /*var interruptEnable = _mainMemory.ReadByte(MiscRegisters.c_InterruptEnable);
                var interruptFlags = _mainMemory.ReadByte(MiscRegisters.c_InterruptEnable);

                if (interruptEnable == 1) //change this
                    LoadNextInterruptRoutine();
                else*/
                    LoadNextInstruction();
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

            _currentInstruction = null;

            //set initial memory registers
           
        }

        private void LoadNextInstruction()
        {
            var nextOpcode = _mainMemory.ReadByte(_cpuState.ProgramCounter++);

            if (_cpuState.InstructionPrefix)
            {
                _currentInstruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(nextOpcode);
                _cpuState.InstructionPrefix = false;
            }
            else
                _currentInstruction = InstructionDecoder.GetInstructionFromOpcode(nextOpcode);
        }

        private void LoadNextInterruptRoutine()
        {

        }
    }
}

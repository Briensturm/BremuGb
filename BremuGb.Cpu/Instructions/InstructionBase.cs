using System;

using BremuGb.Memory;

namespace BremuGb.Cpu.Instructions
{
    public abstract class InstructionBase : IInstruction
    {
        protected int _remainingCycles;
        protected byte _opcode;
        protected abstract int InstructionLength { get; }

        public virtual void ExecuteCycle(ICpuState cpuState, IRandomAccessMemory mainMemory)
        {
            if (cpuState == null)
                throw new ArgumentNullException(nameof(cpuState));
            if (mainMemory == null)
                throw new ArgumentNullException(nameof(mainMemory));

            //caller needs to check IsFetchNecessary() beforehand
            if (IsFetchNecessary())
                throw new InvalidOperationException("No remaining cycles for this instruction");

            _remainingCycles--;
        }

        public bool IsFetchNecessary()
        {
            return _remainingCycles == 0;
        }

        public void Initialize(byte opcode = 0x00)
        {
            _remainingCycles = InstructionLength;
            _opcode = opcode;
        }

        protected bool IsConditionMet(ICpuState cpuState)
        {
            var condition = _opcode & 0x18;
            return condition switch
            {
                0x00 => !cpuState.Registers.ZeroFlag,
                0x08 => cpuState.Registers.ZeroFlag,
                0x10 => !cpuState.Registers.CarryFlag,
                0x18 => cpuState.Registers.CarryFlag,
                _ => throw new InvalidOperationException($"Unexpected behavior for conditional opcode 0x{_opcode:X2} with condition 0x{condition:X2}"),
            };
        }
    }
}

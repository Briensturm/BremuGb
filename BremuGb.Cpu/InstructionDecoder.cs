using System;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu
{
    public static class InstructionDecoder
    {
        public static IInstruction GetInstructionFromOpcode(byte opcode)
        {
            switch(opcode)
            {
                case 0x00:
                    return new TestInstruction();
                default:
                    throw new InvalidOperationException("Unknown opcode, unable to decode: " + opcode);
            }
        }
    }
}

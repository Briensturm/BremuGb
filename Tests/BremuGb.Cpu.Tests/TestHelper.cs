using BremuGb.Cpu.Instructions;
using NUnit.Framework;
using System;

namespace BremuGb.Cpu.Tests
{
    internal static class TestHelper
    {
        internal static void AssertCpuState(ICpuState expectedState, ICpuState actualState)
        {
            Assert.AreEqual(expectedState.StackPointer, actualState.StackPointer, "Stack pointer does not contain the expected address");
            Assert.AreEqual(expectedState.ProgramCounter, actualState.ProgramCounter, "Program counter does not contain the expected address");

            Assert.AreEqual(expectedState.Registers.BC, actualState.Registers.BC, "BC does not contain the expected data");
            Assert.AreEqual(expectedState.Registers.HL, actualState.Registers.HL, "HL does not contan the expected data");
            Assert.AreEqual(expectedState.Registers.DE, actualState.Registers.DE, "DE does not contain the expected data");
            Assert.AreEqual(expectedState.Registers.A, actualState.Registers.A, "Accumulator does not contain the expected data");
            Assert.AreEqual(expectedState.Registers.F, actualState.Registers.F, "Flags are not set according to expectation");

            Assert.AreEqual(expectedState.HaltMode, actualState.HaltMode, "Halt mode is not set according to expectation");
            Assert.AreEqual(expectedState.StopMode, actualState.StopMode, "Stop mode is not set according to expectation");
            Assert.AreEqual(expectedState.InstructionPrefix, actualState.InstructionPrefix, "Prefix is not set according to expectation");
            Assert.AreEqual(expectedState.InterruptMasterEnable, actualState.InterruptMasterEnable, "IME is not set according to expectation");
            Assert.AreEqual(expectedState.ImeScheduled, actualState.ImeScheduled, "ImeScheduled is not set according to expectation");
        }
    }
}

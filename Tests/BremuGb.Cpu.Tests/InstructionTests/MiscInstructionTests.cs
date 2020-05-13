using NUnit.Framework;
using Moq;

using BremuGb.Memory;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class MiscInstructionTests
    {
        [Test]
        public void Test_NOP()
        {
            var expectedState = new CpuState();
            var actualState = new CpuState();
            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new NOP();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_DAA()
        {
            byte a = 0x22;

            var actualState = new CpuState();
            actualState.Registers.A = a;
            actualState.Registers.HalfCarryFlag = true;
            
            var expectedState = new CpuState();
            expectedState.Registers.A = 0x28;
            
            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new DAA();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_SCF()
        {
            var expectedState = new CpuState();
            expectedState.Registers.CarryFlag = true;

            var actualState = new CpuState();
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.SubtractionFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SCF();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_HALT()
        {
            var expectedState = new CpuState();
            expectedState.HaltMode = true;

            var actualState = new CpuState();
            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new HALT();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_STOP()
        {
            var expectedState = new CpuState();
            expectedState.StopMode = true;

            var actualState = new CpuState();
            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new STOP();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_PREFIX()
        {
            var expectedState = new CpuState();
            expectedState.InstructionPrefix = true;

            var actualState = new CpuState();
            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new PREFIX();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_DI()
        {
            var expectedState = new CpuState();
            var actualState = new CpuState
            {
                InterruptMasterEnable = true,
                ImeScheduled = true
            };

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new DI();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_EI()
        {
            var expectedState = new CpuState
            {
                ImeScheduled = true
            };

            var actualState = new CpuState();

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new EI();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_CCF()
        {
            var expectedState = new CpuState();
            expectedState.Registers.CarryFlag = !expectedState.Registers.CarryFlag;

            var actualState = new CpuState();
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.SubtractionFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new CCF();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_CPL()
        {
            var expectedState = new CpuState();
            expectedState.Registers.A = 0x42;
            expectedState.Registers.HalfCarryFlag = true;
            expectedState.Registers.SubtractionFlag = true;

            var actualState = new CpuState();
            actualState.Registers.A = (byte)(~expectedState.Registers.A);

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new CPL();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }
    }
}
using NUnit.Framework;
using Moq;

using BremuGb.Memory;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class StackInstructionTests
    {
        [TestCase(0xC5)]
        [TestCase(0xD5)]
        [TestCase(0xE5)]
        [TestCase(0xF5)]
        public void Test_PUSH(byte opcode)
        {
            ushort sp = 0x4242;

            var expectedState = new CpuState();
            expectedState.StackPointer = (ushort)(sp - 2);

            var actualState = new CpuState();
            actualState.StackPointer = sp;

            actualState.Registers.BC = 0x1111;
            expectedState.Registers.BC = 0x1111;
            actualState.Registers.DE = 0x2222;
            expectedState.Registers.DE = 0x2222;
            actualState.Registers.HL = 0x3333;
            expectedState.Registers.HL = 0x3333;
            expectedState.Registers.F = 0x44;
            expectedState.Registers.A = 0x55;
            actualState.Registers.F = 0x44;
            actualState.Registers.A = 0x55;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new PUSH(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);

            switch (opcode)
            {
                case 0xC5:
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 1), actualState.Registers.B), Times.Once);
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 2), actualState.Registers.C), Times.Once);
                    break;
                case 0xD5:
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 1), actualState.Registers.D), Times.Once);
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 2), actualState.Registers.E), Times.Once);
                    break;
                case 0xE5:
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 1), actualState.Registers.H), Times.Once);
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 2), actualState.Registers.L), Times.Once);
                    break;
                case 0xF5:
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 1), actualState.Registers.A), Times.Once);
                    memoryMock.Verify(m => m.WriteByte((ushort)(sp - 2), actualState.Registers.F), Times.Once);
                    break;
            }
        }

        [TestCase(0xC1)]
        [TestCase(0xD1)]
        [TestCase(0xE1)]
        [TestCase(0xF1)]
        public void Test_POP(byte opcode)
        {
            ushort sp = 0x4242;
            byte lsbData = 0xFF;
            byte msbData = 0x11;

            var expectedState = new CpuState();
            expectedState.StackPointer = (ushort)(sp + 2);

            switch (opcode)
            {
                case 0xC1:
                    expectedState.Registers.BC = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0xD1:
                    expectedState.Registers.DE = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0xE1:
                    expectedState.Registers.HL = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0xF1:
                    expectedState.Registers.F = lsbData;
                    expectedState.Registers.A = msbData;
                    break;
            }

            var actualState = new CpuState();
            actualState.StackPointer = sp;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(sp)).Returns(lsbData);
            memoryMock.Setup(m => m.ReadByte((ushort)(sp + 1))).Returns(msbData);

            var instruction = new POP(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }
    }
}
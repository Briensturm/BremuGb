using NUnit.Framework;
using Moq;

using BremuGb.Memory;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class RotateShiftInstructionTests
    {
        [TestCase(0x00)]
        [TestCase(0x01)]
        [TestCase(0x02)]
        [TestCase(0x03)]
        [TestCase(0x04)]
        [TestCase(0x05)]
        [TestCase(0x07)]
        public void Test_RLCR8(byte opcode)
        {
            var actualState = new CpuState();
            actualState.Registers[opcode] = 0x80;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[opcode] = 0x01;
            expectedState.Registers.CarryFlag = true;            

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RLCR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_RLCA()
        {
            var actualState = new CpuState();
            actualState.Registers.A = 0x80;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers.A = 0x01;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RLCA();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x10)]
        [TestCase(0x11)]
        [TestCase(0x12)]
        [TestCase(0x13)]
        [TestCase(0x14)]
        [TestCase(0x15)]
        [TestCase(0x17)]
        public void Test_RLR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x08;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x11;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RLR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_RLA()
        {
            var actualState = new CpuState();
            actualState.Registers.A = 0x08;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers.A = 0x11;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RLA();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x08)]
        [TestCase(0x09)]
        [TestCase(0x0A)]
        [TestCase(0x0B)]
        [TestCase(0x0C)]
        [TestCase(0x0D)]
        [TestCase(0x0F)]
        public void Test_RRCR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x01;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x80;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RRCR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_RRCA()
        {
            var actualState = new CpuState();
            actualState.Registers.A = 0x01;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers.A = 0x80;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RRCA();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x18)]
        [TestCase(0x19)]
        [TestCase(0x1A)]
        [TestCase(0x1B)]
        [TestCase(0x1C)]
        [TestCase(0x1D)]
        [TestCase(0x1F)]
        public void Test_RRR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x81;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x40;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RRR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_RRA()
        {
            var actualState = new CpuState();
            actualState.Registers.A = 0x81;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers.A = 0x40;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RRA();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x20)]
        [TestCase(0x21)]
        [TestCase(0x22)]
        [TestCase(0x23)]
        [TestCase(0x24)]
        [TestCase(0x25)]
        [TestCase(0x27)]
        public void Test_SLAR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x01;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x02;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SLAR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x28)]
        [TestCase(0x29)]
        [TestCase(0x2A)]
        [TestCase(0x2B)]
        [TestCase(0x2C)]
        [TestCase(0x2D)]
        [TestCase(0x2F)]
        public void Test_SRAR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0xC1;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0xE0;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SRAR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x38)]
        [TestCase(0x39)]
        [TestCase(0x3A)]
        [TestCase(0x3B)]
        [TestCase(0x3C)]
        [TestCase(0x3D)]
        [TestCase(0x3F)]
        public void Test_SRLR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x80;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x40;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SRLR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x30)]
        [TestCase(0x31)]
        [TestCase(0x32)]
        [TestCase(0x33)]
        [TestCase(0x34)]
        [TestCase(0x35)]
        [TestCase(0x37)]
        public void Test_SWAPR8(byte opcode)
        {
            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = 0x42;
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = 0x24;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SWAPR8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_RL_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x08;
            byte expectedData = 0x11;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new RL_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_RLC_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x80;
            byte expectedData = 0x01;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;            
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new RLC_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_RR_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x81;
            byte expectedData = 0x40;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new RR_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_RRC_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x01;
            byte expectedData = 0x80;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new RRC_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_SLA_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x01;
            byte expectedData = 0x02;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;            

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new SLA_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_SRA_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0xC1;
            byte expectedData = 0xE0;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;            
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.Registers.CarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new SRA_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_SRL_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x80;
            byte expectedData = 0x40;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;            

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new SRL_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }

        [Test]
        public void Test_SWAP_HL_()
        {
            ushort hl = 0x4242;
            byte actualData = 0x42;
            byte expectedData = 0x24;

            var actualState = new CpuState();
            actualState.Registers.ZeroFlag = true;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.HalfCarryFlag = true;
            actualState.Registers.CarryFlag = true;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(actualData);

            var instruction = new SWAP_HL_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, expectedData), Times.Once);
        }
    }
}
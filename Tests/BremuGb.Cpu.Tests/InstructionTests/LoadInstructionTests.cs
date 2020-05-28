using NUnit.Framework;
using Moq;

using BremuGb.Memory;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class LoadInstructionTests
    {
        [TestCase(0x40)]
        [TestCase(0x50)]
        [TestCase(0x60)]
        [TestCase(0x41)]
        [TestCase(0x51)]
        [TestCase(0x61)]
        [TestCase(0x42)]
        [TestCase(0x52)]
        [TestCase(0x62)]
        [TestCase(0x43)]
        [TestCase(0x53)]
        [TestCase(0x63)]
        [TestCase(0x44)]
        [TestCase(0x54)]
        [TestCase(0x64)]
        [TestCase(0x45)]
        [TestCase(0x55)]
        [TestCase(0x65)]
        [TestCase(0x47)]
        [TestCase(0x57)]
        [TestCase(0x67)]
        [TestCase(0x48)]
        [TestCase(0x58)]
        [TestCase(0x68)]
        [TestCase(0x78)]
        [TestCase(0x49)]
        [TestCase(0x59)]
        [TestCase(0x69)]
        [TestCase(0x79)]
        [TestCase(0x4A)]
        [TestCase(0x5A)]
        [TestCase(0x6A)]
        [TestCase(0x7A)]
        [TestCase(0x4B)]
        [TestCase(0x5B)]
        [TestCase(0x6B)]
        [TestCase(0x7B)]
        [TestCase(0x4C)]
        [TestCase(0x5C)]
        [TestCase(0x6C)]
        [TestCase(0x7C)]
        [TestCase(0x4D)]
        [TestCase(0x5D)]
        [TestCase(0x6D)]
        [TestCase(0x7D)]
        [TestCase(0x4F)]
        [TestCase(0x5F)]
        [TestCase(0x6F)]
        [TestCase(0x7F)]
        public void Test_LDR8R8(byte opcode)
        {
            byte data = 0x42;

            var sourceIndex = opcode & 0x07;
            var targetIndex = (opcode >> 3) & 0x07;

            var expectedState = new CpuState();
            expectedState.Registers[sourceIndex] = data;
            expectedState.Registers[targetIndex] = data;

            var actualState = new CpuState();
            actualState.Registers[sourceIndex] = data;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LDR8R8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x01)]
        [TestCase(0x11)]
        [TestCase(0x21)]
        [TestCase(0x31)]
        public void Test_LDR16N16(byte opcode)
        {
            ushort pc = 0x1122;
            byte lsbData = 0x24;
            byte msbData = 0x42;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.ProgramCounter = (ushort)(actualState.ProgramCounter + 2); 
            
            switch(opcode)
            {
                case 0x01:
                    expectedState.Registers.BC = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0x11:
                    expectedState.Registers.DE = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0x21:
                    expectedState.Registers.HL = (ushort)((msbData << 8) | lsbData);
                    break;
                case 0x31:
                    expectedState.StackPointer = (ushort)((msbData << 8) | lsbData);
                    break;
            }

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(lsbData);
            memoryMock.Setup(m => m.ReadByte((ushort)(pc + 1))).Returns(msbData);

            var instruction = new LDR16D16();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LD_HL_D8()
        {
            ushort pc = 0x1234;
            ushort hl = 0x4321;
            byte data = 0x42;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.ProgramCounter = (ushort)(actualState.ProgramCounter + 1);
            expectedState.Registers.HL = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(data);

            var instruction = new LD_HL_D8();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, data), Times.Once);
        }

        [Test]
        public void Test_LD_D16_SP()
        {
            ushort pc = 0x1234;
            ushort sp = 0x4321;
            byte addressLsb = 0x42;
            byte addressMsb = 0x43;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;
            actualState.StackPointer = sp;

            var expectedState = new CpuState();
            expectedState.ProgramCounter = (ushort)(actualState.ProgramCounter + 2);
            expectedState.StackPointer = sp;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(addressLsb);
            memoryMock.Setup(m => m.ReadByte((ushort)(pc+1))).Returns(addressMsb);

            var instruction = new LD_D16_SP();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);

            ushort targetAddress = (ushort)(addressLsb | (addressMsb << 8));
            memoryMock.Verify(m => m.WriteByte(targetAddress, (byte)(expectedState.StackPointer)), Times.Once);
            memoryMock.Verify(m => m.WriteByte((ushort)(targetAddress+1), (byte)(expectedState.StackPointer >> 8)), Times.Once);
        }

        [Test]
        public void Test_LDSPHL()
        {
            ushort hl = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.StackPointer = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LDSPHL();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x06)]
        [TestCase(0x16)]
        [TestCase(0x26)]
        [TestCase(0x0E)]
        [TestCase(0x1E)]
        [TestCase(0x2E)]
        [TestCase(0x3E)]
        public void Test_LDR8D8(byte opcode)
        {
            ushort pc = 0x1234;
            byte data = 0x42;

            var registerIndex = opcode >> 3;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.ProgramCounter = (ushort)(actualState.ProgramCounter + 1);
            expectedState.Registers[registerIndex] = data;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(data);

            var instruction = new LDR8D8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x70)]
        [TestCase(0x71)]
        [TestCase(0x72)]
        [TestCase(0x73)]
        [TestCase(0x74)]
        [TestCase(0x75)]
        [TestCase(0x77)]
        public void Test_LD_HL_R8(byte opcode)
        {
            ushort hl = 0x1212;
            byte data = 0x12;

            var registerIndex = opcode & 0x07;

            var actualState = new CpuState();
            actualState.Registers[registerIndex] = data;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = data;
            expectedState.Registers.HL = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LD_HL_R8();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, data), Times.Once);
        }

        [TestCase(0x46)]
        [TestCase(0x56)]
        [TestCase(0x66)]
        [TestCase(0x4E)]
        [TestCase(0x5E)]
        [TestCase(0x6E)]
        [TestCase(0x7E)]
        public void Test_LDR8_HL_(byte opcode)
        {
            ushort hl = 0x1212;
            byte data = 0x12;

            var registerIndex = (opcode >> 3) & 0x7;

            var actualState = new CpuState();            
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers[registerIndex] = data;
            expectedState.Registers.HL = hl;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new LDR8_HL_();
            instruction.Initialize(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LD_BC_A()
        {
            byte data = 0x42;
            ushort bc = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.Registers.BC = bc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.BC = bc;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LD_BC_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(bc, data), Times.Once);
        }

        [Test]
        public void Test_LD_DE_A()
        {
            byte data = 0x42;
            ushort de = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.Registers.DE = de;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.DE = de;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LD_DE_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(de, data), Times.Once);
        }

        [Test]
        public void Test_LD_HLP_A()
        {
            byte data = 0x42;
            ushort hl = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.HL = (ushort)(hl + 1);

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LD_HLP_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, data), Times.Once);
        }

        [Test]
        public void Test_LD_HLM_A()
        {
            byte data = 0x42;
            ushort hl = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.HL = (ushort)(hl - 1);

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LD_HLM_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, data), Times.Once);
        }

        [Test]
        public void Test_LDA_BC_()
        {
            byte data = 0x42;
            ushort bc = 0x1234;

            var actualState = new CpuState();            
            actualState.Registers.BC = bc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.BC = bc;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(bc)).Returns(data);

            var instruction = new LDA_BC_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDA_DE_()
        {
            byte data = 0x42;
            ushort de = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.DE = de;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.DE = de;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(de)).Returns(data);

            var instruction = new LDA_DE_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDA_HLP_()
        {
            byte data = 0x42;
            ushort hl = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.HL = (ushort)(hl + 1);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new LDA_HLP_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDA_HLM_()
        {
            byte data = 0x42;
            ushort hl = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.HL = (ushort)(hl - 1);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new LDA_HLM_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDH_D8_A()
        {
            byte data = 0x42;
            byte addressLsb = 0x24;
            ushort pc = 0x1234;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.ProgramCounter = (ushort)(pc + 1);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(addressLsb);

            var instruction = new LDH_D8_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte((ushort)((0xFF << 8) | addressLsb), data), Times.Once);
        }

        [Test]
        public void Test_LDHA_D8_()
        {
            byte data = 0x42;
            byte addressLsb = 0x24;
            ushort pc = 0x1234;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.ProgramCounter = (ushort)(pc + 1);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(addressLsb);
            memoryMock.Setup(m => m.ReadByte((ushort)((0xFF << 8) | addressLsb))).Returns(data);

            var instruction = new LDHA_D8_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDH_C_A()
        {
            byte data = 0x42;
            byte addressLsb = 0x24;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.Registers.C = addressLsb;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.C = addressLsb;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new LDH_C_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte((ushort)((0xFF << 8) | addressLsb), data), Times.Once);
        }

        [Test]
        public void Test_LDHA_C_()
        {
            byte data = 0x42;
            byte addressLsb = 0x24;

            var actualState = new CpuState();
            actualState.Registers.C = addressLsb;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.Registers.C = addressLsb;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte((ushort)((0xFF << 8) | addressLsb))).Returns(data);

            var instruction = new LDHA_C_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LD_D16_A()
        {
            ushort pc = 0x1111;
            byte data = 0x42;
            byte addressLsb = 0x23;
            byte addressMsb = 0x01;

            var actualState = new CpuState();
            actualState.Registers.A = data;
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.ProgramCounter = (ushort)(pc + 2);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(addressLsb);
            memoryMock.Setup(m => m.ReadByte((ushort)(pc + 1))).Returns(addressMsb);

            var instruction = new LD_D16_A();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte((ushort)((addressMsb << 8) | addressLsb), data), Times.Once);
        }

        [Test]
        public void Test_LDA_D16_()
        {
            ushort pc = 0x1111;
            byte data = 0x42;
            byte addressLsb = 0x23;
            byte addressMsb = 0x01;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;

            var expectedState = new CpuState();
            expectedState.Registers.A = data;
            expectedState.ProgramCounter = (ushort)(pc + 2);

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(addressLsb);
            memoryMock.Setup(m => m.ReadByte((ushort)(pc + 1))).Returns(addressMsb);
            memoryMock.Setup(m => m.ReadByte((ushort)((addressMsb << 8) | addressLsb))).Returns(data);

            var instruction = new LDA_D16_();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Test_LDHLSPS8()
        {
            ushort pc = 0x1111;
            ushort sp = 0xFFFF;
            byte data = 0x1;

            var actualState = new CpuState();
            actualState.ProgramCounter = pc;
            actualState.StackPointer = sp;
            actualState.Registers.SubtractionFlag = true;
            actualState.Registers.ZeroFlag = true;

            var expectedState = new CpuState();
            expectedState.Registers.HL = (ushort)(sp + (sbyte)data);
            expectedState.StackPointer = sp;
            expectedState.ProgramCounter = (ushort)(pc + 1);
            expectedState.Registers.CarryFlag = true;
            expectedState.Registers.HalfCarryFlag = true;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(pc)).Returns(data);

            var instruction = new LDHLSPS8();
            instruction.Initialize();

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }
    }
}
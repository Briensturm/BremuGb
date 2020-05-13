using NUnit.Framework;
using Moq;

using BremuGb.Memory;
using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class BitInstructionTests
    {
        [TestCase(0x40)]
        [TestCase(0x41)]
        [TestCase(0x42)]
        [TestCase(0x43)]
        [TestCase(0x44)]
        [TestCase(0x45)]
        [TestCase(0x47)]
        [TestCase(0x48)]
        [TestCase(0x49)]
        [TestCase(0x4A)]
        [TestCase(0x4B)]
        [TestCase(0x4C)]
        [TestCase(0x4D)]
        [TestCase(0x4F)]
        [TestCase(0x50)]
        [TestCase(0x51)]
        [TestCase(0x52)]
        [TestCase(0x53)]
        [TestCase(0x54)]
        [TestCase(0x55)]
        [TestCase(0x57)]
        [TestCase(0x58)]
        [TestCase(0x59)]
        [TestCase(0x5A)]
        [TestCase(0x5B)]
        [TestCase(0x5C)]
        [TestCase(0x5D)]
        [TestCase(0x5F)]
        [TestCase(0x60)]
        [TestCase(0x61)]
        [TestCase(0x62)]
        [TestCase(0x63)]
        [TestCase(0x64)]
        [TestCase(0x65)]
        [TestCase(0x67)]
        [TestCase(0x68)]
        [TestCase(0x69)]
        [TestCase(0x6A)]
        [TestCase(0x6B)]
        [TestCase(0x6C)]
        [TestCase(0x6D)]
        [TestCase(0x6F)]
        [TestCase(0x70)]
        [TestCase(0x71)]
        [TestCase(0x72)]
        [TestCase(0x73)]
        [TestCase(0x74)]
        [TestCase(0x75)]
        [TestCase(0x77)]
        [TestCase(0x78)]
        [TestCase(0x79)]
        [TestCase(0x7A)]
        [TestCase(0x7B)]
        [TestCase(0x7C)]
        [TestCase(0x7D)]
        [TestCase(0x7F)]
        public void Test_BITND8(byte opcode)
        {
            var expectedState = new CpuState();
            expectedState.Registers.BC = 0x1234;
            expectedState.Registers.DE = 0x5678;
            expectedState.Registers.HL = 0x9ABC;
            expectedState.Registers.A = 0xDE;
            expectedState.Registers.HalfCarryFlag = true;

            var actualState = new CpuState();
            actualState.Registers.BC = 0x1234;
            actualState.Registers.DE = 0x5678;
            actualState.Registers.HL = 0x9ABC;
            actualState.Registers.A = 0xDE;
            actualState.Registers.SubtractionFlag = true;

            var bitIndex = (opcode & 0x38) >> 3;
            var registerIndex = opcode & 0x07;

            expectedState.Registers.ZeroFlag = ((byte)(actualState.Registers[registerIndex] >> bitIndex) & 0x01) == 0;

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new BITNR8(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [Test, Combinatorial]
        public void Test_BITN_HL_([Values(0x46, 0x56, 0x66, 0x76, 0x4E, 0x5E, 0x6E, 0x7E)] byte opcode,
                              [Values(0x00, 0xFF)] byte data)
        {
            ushort hl = 0x4242;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;
            expectedState.Registers.HalfCarryFlag = true;


            var actualState = new CpuState();
            actualState.Registers.HL = hl;
            actualState.Registers.SubtractionFlag = true;

            var bitIndex = (opcode & 0x38) >> 3;
            var registerIndex = opcode & 0x07;

            expectedState.Registers.ZeroFlag = ((byte)(data >> bitIndex) & 0x01) == 0;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new BITN_HL_(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0xC0)]
        [TestCase(0xC1)]
        [TestCase(0xC2)]
        [TestCase(0xC3)]
        [TestCase(0xC4)]
        [TestCase(0xC5)]
        [TestCase(0xC7)]
        [TestCase(0xC8)]
        [TestCase(0xC9)]
        [TestCase(0xCA)]
        [TestCase(0xCB)]
        [TestCase(0xCC)]
        [TestCase(0xCD)]
        [TestCase(0xCF)]
        [TestCase(0xD0)]
        [TestCase(0xD1)]
        [TestCase(0xD2)]
        [TestCase(0xD3)]
        [TestCase(0xD4)]
        [TestCase(0xD5)]
        [TestCase(0xD7)]
        [TestCase(0xD8)]
        [TestCase(0xD9)]
        [TestCase(0xDA)]
        [TestCase(0xDB)]
        [TestCase(0xDC)]
        [TestCase(0xDD)]
        [TestCase(0xDF)]
        [TestCase(0xE0)]
        [TestCase(0xE1)]
        [TestCase(0xE2)]
        [TestCase(0xE3)]
        [TestCase(0xE4)]
        [TestCase(0xE5)]
        [TestCase(0xE7)]
        [TestCase(0xE8)]
        [TestCase(0xE9)]
        [TestCase(0xEA)]
        [TestCase(0xEC)]
        [TestCase(0xED)]
        [TestCase(0xEF)]
        [TestCase(0xF0)]
        [TestCase(0xF1)]
        [TestCase(0xF2)]
        [TestCase(0xF3)]
        [TestCase(0xF4)]
        [TestCase(0xF5)]
        [TestCase(0xF7)]
        [TestCase(0xF8)]
        [TestCase(0xF9)]
        [TestCase(0xFA)]
        [TestCase(0xFB)]
        [TestCase(0xFC)]
        [TestCase(0xFD)]
        [TestCase(0xFF)]
        public void Test_SETNR8(byte opcode)
        {
            var expectedState = new CpuState();

            var actualState = new CpuState();

            var bitIndex = (opcode & 0x38) >> 3;
            var registerIndex = opcode & 0x07;

            expectedState.Registers[registerIndex] = (ushort)(0x01 << bitIndex);

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new SETNR8(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x80)]
        [TestCase(0x81)]
        [TestCase(0x82)]
        [TestCase(0x83)]
        [TestCase(0x84)]
        [TestCase(0x85)]
        [TestCase(0x87)]
        [TestCase(0x88)]
        [TestCase(0x89)]
        [TestCase(0x8A)]
        [TestCase(0x8B)]
        [TestCase(0x8C)]
        [TestCase(0x8D)]
        [TestCase(0x8F)]
        [TestCase(0x90)]
        [TestCase(0x91)]
        [TestCase(0x92)]
        [TestCase(0x93)]
        [TestCase(0x94)]
        [TestCase(0x95)]
        [TestCase(0x97)]
        [TestCase(0x98)]
        [TestCase(0x99)]
        [TestCase(0x9A)]
        [TestCase(0x9B)]
        [TestCase(0x9C)]
        [TestCase(0x9D)]
        [TestCase(0x9F)]
        [TestCase(0xA0)]
        [TestCase(0xA1)]
        [TestCase(0xA2)]
        [TestCase(0xA3)]
        [TestCase(0xA4)]
        [TestCase(0xA5)]
        [TestCase(0xA7)]
        [TestCase(0xA8)]
        [TestCase(0xA9)]
        [TestCase(0xAA)]
        [TestCase(0xAC)]
        [TestCase(0xAD)]
        [TestCase(0xAF)]
        [TestCase(0xB0)]
        [TestCase(0xB1)]
        [TestCase(0xB2)]
        [TestCase(0xB3)]
        [TestCase(0xB4)]
        [TestCase(0xB5)]
        [TestCase(0xB7)]
        [TestCase(0xB8)]
        [TestCase(0xB9)]
        [TestCase(0xBA)]
        [TestCase(0xBB)]
        [TestCase(0xBC)]
        [TestCase(0xBD)]
        [TestCase(0xBF)]
        public void Test_RESNR8(byte opcode)
        {
            var expectedState = new CpuState();
            expectedState.Registers.BC = 0xFFFF;
            expectedState.Registers.DE = 0xFFFF;
            expectedState.Registers.HL = 0xFFFF;
            expectedState.Registers.A = 0xFF;

            var actualState = new CpuState();
            actualState.Registers.BC = 0xFFFF;
            actualState.Registers.DE = 0xFFFF;
            actualState.Registers.HL = 0xFFFF;
            actualState.Registers.A = 0xFF;

            var bitIndex = (opcode & 0x38) >> 3;
            var registerIndex = opcode & 0x07;

            expectedState.Registers[registerIndex] = (ushort)(expectedState.Registers[registerIndex] & ~(0x01 << bitIndex));

            var memoryMock = new Mock<IRandomAccessMemory>();

            var instruction = new RESNR8(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        [TestCase(0x86)]
        [TestCase(0x96)]
        [TestCase(0xA6)]
        [TestCase(0xB6)]
        [TestCase(0x8E)]
        [TestCase(0x9E)]
        [TestCase(0xAE)]
        [TestCase(0xBE)]
        public void Test_RESN_HL_(byte opcode)
        {
            byte data = 0xFF;
            ushort hl = 0x4242;           

            var actualState = new CpuState();
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;

            var bitIndex = (opcode & 0x38) >> 3;            

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new RESN_HL_(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, (byte)(data & ~(0x01 << bitIndex))), Times.Once);
        }

        [TestCase(0xC6)]
        [TestCase(0xD6)]
        [TestCase(0xE6)]
        [TestCase(0xF6)]
        [TestCase(0xCE)]
        [TestCase(0xDE)]
        [TestCase(0xEE)]
        [TestCase(0xFE)]
        public void Test_SETN_HL_(byte opcode)
        {
            byte data = 0x00;
            ushort hl = 0x4242;

            var actualState = new CpuState();
            actualState.Registers.HL = hl;

            var expectedState = new CpuState();
            expectedState.Registers.HL = hl;

            var bitIndex = (opcode & 0x38) >> 3;

            var memoryMock = new Mock<IRandomAccessMemory>();
            memoryMock.Setup(m => m.ReadByte(hl)).Returns(data);

            var instruction = new SETN_HL_(opcode);

            //act
            while (!instruction.IsFetchNecessary())
                instruction.ExecuteCycle(actualState, memoryMock.Object);

            //assert
            TestHelper.AssertCpuState(expectedState, actualState);
            memoryMock.Verify(m => m.WriteByte(hl, (byte)(data | (0x01 << bitIndex))), Times.Once);
        }
    }
}
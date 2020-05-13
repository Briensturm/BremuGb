using NUnit.Framework;

using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class PrefixedInstructionDecoderTests
    {
        [Test]
        public void All_prefixed_instructions_are_implemented()
        {
            for (ushort opcode = 0x00; opcode <= 0xFF; opcode++)
            {
                Assert.DoesNotThrow(() => InstructionDecoder.GetPrefixedInstructionFromOpcode((byte)opcode));
            }
        }

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
        public void Decode_BITNR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<BITNR8>(instruction);
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
        public void Decode_SETNR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SETNR8>(instruction);
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
        public void Decode_RESNR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RESNR8>(instruction);
        }

        [TestCase(0x86)]
        [TestCase(0x96)]
        [TestCase(0xA6)]
        [TestCase(0xB6)]
        [TestCase(0x8E)]
        [TestCase(0x9E)]
        [TestCase(0xAE)]
        [TestCase(0xBE)]
        public void Decode_RESN_HL_(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RESN_HL_>(instruction);
        }

        [TestCase(0xC6)]
        [TestCase(0xD6)]
        [TestCase(0xE6)]
        [TestCase(0xF6)]
        [TestCase(0xCE)]
        [TestCase(0xDE)]
        [TestCase(0xEE)]
        [TestCase(0xFE)]
        public void Decode_SETN_HL_(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SETN_HL_>(instruction);
        }

        [TestCase(0x46)]
        [TestCase(0x56)]
        [TestCase(0x66)]
        [TestCase(0x76)]
        [TestCase(0x4E)]
        [TestCase(0x5E)]
        [TestCase(0x6E)]
        [TestCase(0x7E)]
        public void Decode_BITN_HL_(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<BITN_HL_>(instruction);
        }

        [TestCase(0x00)]
        [TestCase(0x01)]
        [TestCase(0x02)]
        [TestCase(0x03)]
        [TestCase(0x04)]
        [TestCase(0x05)]
        [TestCase(0x07)]
        public void Decode_RLCR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RLCR8>(instruction);
        }

        [TestCase(0x10)]
        [TestCase(0x11)]
        [TestCase(0x12)]
        [TestCase(0x13)]
        [TestCase(0x14)]
        [TestCase(0x15)]
        [TestCase(0x17)]
        public void Decode_RLR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RLR8>(instruction);
        }

        [TestCase(0x08)]
        [TestCase(0x09)]
        [TestCase(0x0A)]
        [TestCase(0x0B)]
        [TestCase(0x0C)]
        [TestCase(0x0D)]
        [TestCase(0x0F)]
        public void Decode_RRCR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RRCR8>(instruction);
        }

        [TestCase(0x18)]
        [TestCase(0x19)]
        [TestCase(0x1A)]
        [TestCase(0x1B)]
        [TestCase(0x1C)]
        [TestCase(0x1D)]
        [TestCase(0x1F)]
        public void Decode_RRR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RRR8>(instruction);
        }

        [TestCase(0x20)]
        [TestCase(0x21)]
        [TestCase(0x22)]
        [TestCase(0x23)]
        [TestCase(0x24)]
        [TestCase(0x25)]
        [TestCase(0x27)]
        public void Decode_SLAR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SLAR8>(instruction);
        }

        [TestCase(0x28)]
        [TestCase(0x29)]
        [TestCase(0x2A)]
        [TestCase(0x2B)]
        [TestCase(0x2C)]
        [TestCase(0x2D)]
        [TestCase(0x2F)]
        public void Decode_SRAR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SRAR8>(instruction);
        }

        [TestCase(0x38)]
        [TestCase(0x39)]
        [TestCase(0x3A)]
        [TestCase(0x3B)]
        [TestCase(0x3C)]
        [TestCase(0x3D)]
        [TestCase(0x3F)]
        public void Decode_SRLR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SRLR8>(instruction);
        }

        [TestCase(0x30)]
        [TestCase(0x31)]
        [TestCase(0x32)]
        [TestCase(0x33)]
        [TestCase(0x34)]
        [TestCase(0x35)]
        [TestCase(0x37)]
        public void Decode_SWAPR8(byte opcode)
        {
            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SWAPR8>(instruction);
        }

        public void Decode_RL_HL_()
        {
            byte opcode = 0x16;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RL_HL_>(instruction);
        }

        public void Decode_RLC_HL_()
        {
            byte opcode = 0x06;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RLC_HL_>(instruction);
        }

        public void Decode_RR_HL_()
        {
            byte opcode = 0x1E;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RR_HL_>(instruction);
        }

        public void Decode_RRC_HL_()
        {
            byte opcode = 0x0E;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<RRC_HL_>(instruction);
        }

        public void Decode_SLA_HL_()
        {
            byte opcode = 0x26;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SLA_HL_>(instruction);
        }

        public void Decode_SRA_HL_()
        {
            byte opcode = 0x2E;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SRA_HL_>(instruction);
        }

        public void Decode_SRL_HL_()
        {
            byte opcode = 0x3E;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SRL_HL_>(instruction);
        }

        public void Decode_SWAP_HL_()
        {
            byte opcode = 0x36;

            var instruction = InstructionDecoder.GetPrefixedInstructionFromOpcode(opcode);

            Assert.IsInstanceOf<SWAP_HL_>(instruction);
        }
    }
}

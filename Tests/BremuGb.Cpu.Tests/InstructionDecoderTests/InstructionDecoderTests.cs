using System;
using NUnit.Framework;

using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu.Tests
{
    public class InstructionDecoderTests
    {
        InstructionDecoder _instructionDecoder;

        [OneTimeSetUp]
        public void InitTestFixture()
        {
            _instructionDecoder = new InstructionDecoder();
        }

        [Test]
        public void All_non_forbidden_non_prefixed_instructions_are_implemented()
        {
            for (ushort opcode = 0x00; opcode <= 0xFF; opcode++)
            {
                if (opcode == 0xD3 || opcode == 0xE3 || opcode == 0xE4 || opcode == 0xF4
                    || opcode == 0xDB || opcode == 0xEB || opcode == 0xEC || opcode == 0xFC
                    || opcode == 0xDD || opcode == 0xED || opcode == 0xFD)
                    continue;

                Assert.DoesNotThrow(() => _instructionDecoder.DecodeInstruction((byte)opcode, false));
            }
        }

        [Test]
        public void Decode_NOP()
        {
            byte opcode = 0x00;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<NOP>(instruction);
        }

        [Test]
        public void Decode_DAA()
        {
            byte opcode = 0x27;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<DAA>(instruction);
        }

        [Test]
        public void Decode_ADDSPS8()
        {
            byte opcode = 0xE8;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADDSPS8>(instruction);
        }

        [TestCase(0x09)]
        [TestCase(0x19)]
        [TestCase(0x29)]
        [TestCase(0x39)]
        public void Decode_ADDHLR16(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADDHLR16>(instruction);
        }

        [Test]
        public void Decode_SUBAD8()
        {
            byte opcode = 0xD6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SUBAD8>(instruction);
        }

        [Test]
        public void Decode_SUBA_HL_()
        {
            byte opcode = 0x96;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SUBA_HL_>(instruction);
        }

        [TestCase(0x90)]
        [TestCase(0x91)]
        [TestCase(0x92)]
        [TestCase(0x93)]
        [TestCase(0x94)]
        [TestCase(0x95)]
        [TestCase(0x97)]
        public void Decode_SUBAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SUBAR8>(instruction);
        }



        [Test]
        public void Decode_LD_HL_D8()
        {
            byte opcode = 0x36;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_HL_D8>(instruction);
        }

        [Test]
        public void Decode_ADDAD8()
        {
            byte opcode = 0xC6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADDAD8>(instruction);
        }

        [Test]
        public void Decode_ADDA_HL_()
        {
            byte opcode = 0x86;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADDA_HL_>(instruction);
        }

        [TestCase(0x80)]
        [TestCase(0x81)]
        [TestCase(0x82)]
        [TestCase(0x83)]
        [TestCase(0x84)]
        [TestCase(0x85)]
        [TestCase(0x87)]
        public void Decode_ADDAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADDAR8>(instruction);
        }

        [Test]
        public void Decode_ADCAD8()
        {
            byte opcode = 0xCE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADCAD8>(instruction);
        }

        [Test]
        public void Decode_ADCA_HL_()
        {
            byte opcode = 0x8E;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADCA_HL_>(instruction);
        }

        [TestCase(0x88)]
        [TestCase(0x89)]
        [TestCase(0x8A)]
        [TestCase(0x8B)]
        [TestCase(0x8C)]
        [TestCase(0x8D)]
        [TestCase(0x8F)]
        public void Decode_ADCAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ADCAR8>(instruction);
        }

        [Test]
        public void Decode_SBCAD8()
        {
            byte opcode = 0xDE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SBCAD8>(instruction);
        }

        [Test]
        public void Decode_SBCA_HL_()
        {
            byte opcode = 0x9E;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SBCA_HL_>(instruction);
        }

        [TestCase(0x98)]
        [TestCase(0x99)]
        [TestCase(0x9A)]
        [TestCase(0x9B)]
        [TestCase(0x9C)]
        [TestCase(0x9D)]
        [TestCase(0x9F)]
        public void Decode_SBCAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SBCAR8>(instruction);
        }

        [Test]
        public void Decode_CPAD8()
        {
            byte opcode = 0xFE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CPAD8>(instruction);
        }

        [Test]
        public void Decode_CPA_HL_()
        {
            byte opcode = 0xBE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CPA_HL_>(instruction);
        }

        [TestCase(0xB8)]
        [TestCase(0xB9)]
        [TestCase(0xBA)]
        [TestCase(0xBB)]
        [TestCase(0xBC)]
        [TestCase(0xBD)]
        [TestCase(0xBF)]
        public void Decode_CPAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CPAR8>(instruction);
        }

        [TestCase(0x06)]
        [TestCase(0x16)]
        [TestCase(0x26)]
        [TestCase(0x0E)]
        [TestCase(0x1E)]
        [TestCase(0x2E)]
        [TestCase(0x3E)]
        public void Decode_LDR8D8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDR8D8>(instruction);
        }

        [Test]
        public void Decode_INC_HL_()
        {
            byte opcode = 0x34;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<INC_HL_>(instruction);
        }

        [TestCase(0x04)]
        [TestCase(0x14)]
        [TestCase(0x24)]
        [TestCase(0x0C)]
        [TestCase(0x1C)]
        [TestCase(0x2C)]
        [TestCase(0x3C)]
        public void Decode_INCR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<INCR8>(instruction);
        }

        [TestCase(0x03)]
        [TestCase(0x13)]
        [TestCase(0x23)]
        [TestCase(0x33)]
        public void Decode_INCR16(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<INCR16>(instruction);
        }

        [Test]
        public void Decode_DEC_HL_()
        {
            byte opcode = 0x35;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<DEC_HL_>(instruction);
        }

        [TestCase(0x05)]
        [TestCase(0x15)]
        [TestCase(0x25)]
        [TestCase(0x0D)]
        [TestCase(0x1D)]
        [TestCase(0x2D)]
        [TestCase(0x3D)]
        public void Decode_DECR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<DECR8>(instruction);
        }

        [TestCase(0x0B)]
        [TestCase(0x1B)]
        [TestCase(0x2B)]
        [TestCase(0x3B)]
        public void Decode_DECR16(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<DECR16>(instruction);
        }

        [Test]
        public void Decode_CCF()
        {
            byte opcode = 0x3F;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CCF>(instruction);
        }

        [Test]
        public void Decode_CPL()
        {
            byte opcode = 0x2F;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CPL>(instruction);
        }

        [Test]
        public void Decode_DI()
        {
            byte opcode = 0xF3;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<DI>(instruction);
        }

        [Test]
        public void Decode_EI()
        {
            byte opcode = 0xFB;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<EI>(instruction);
        }

        [Test]
        public void Decode_HALT()
        {
            byte opcode = 0x76;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<HALT>(instruction);
        }

        [Test]
        public void Decode_PREFIX()
        {
            byte opcode = 0xCB;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<PREFIX>(instruction);
        }

        [Test]
        public void Decode_STOP()
        {
            byte opcode = 0x10;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<STOP>(instruction);
        }

        [Test]
        public void Decode_SCF()
        {
            byte opcode = 0x37;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<SCF>(instruction);
        }

        [Test]
        public void Decode_CALL()
        {
            byte opcode = 0xCD;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CALL>(instruction);
        }

        [Test]
        public void Decode_JR()
        {
            byte opcode = 0x18;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<JR>(instruction);
        }

        [Test]
        public void Decode_JPHL()
        {
            byte opcode = 0xE9;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<JPHL>(instruction);
        }

        [Test]
        public void Decode_RETI()
        {
            byte opcode = 0xD9;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RETI>(instruction);
        }

        [Test]
        public void Decode_RET()
        {
            byte opcode = 0xC9;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RET>(instruction);
        }

        [TestCase(0xC0)]
        [TestCase(0xD0)]
        [TestCase(0xC8)]
        [TestCase(0xD8)]
        public void Decode_RETCC(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RETCC>(instruction);
        }

        [TestCase(0xC7)]
        [TestCase(0xD7)]
        [TestCase(0xE7)]
        [TestCase(0xF7)]
        [TestCase(0xCF)]
        [TestCase(0xDF)]
        [TestCase(0xEF)]
        [TestCase(0xFF)]
        public void Decode_RST(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RST>(instruction);
        }

        [Test]
        public void Decode_JPNN()
        {
            byte opcode = 0xC3;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<JPD16>(instruction);
        }

        [TestCase(0xC2)]
        [TestCase(0xD2)]
        [TestCase(0xCA)]
        [TestCase(0xDA)]
        public void Decode_JPCC(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<JPCC>(instruction);
        }

        [TestCase(0x20)]
        [TestCase(0x30)]
        [TestCase(0x28)]
        [TestCase(0x38)]
        public void Decode_JRCC(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<JRCC>(instruction);
        }

        [TestCase(0xC1)]
        [TestCase(0xD1)]
        [TestCase(0xE1)]
        [TestCase(0xF1)]
        public void Decode_POP(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<POP>(instruction);
        }

        [TestCase(0xC5)]
        [TestCase(0xD5)]
        [TestCase(0xE5)]
        [TestCase(0xF5)]
        public void Decode_PUSH(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<PUSH>(instruction);
        }

        [TestCase(0xC4)]
        [TestCase(0xD4)]
        [TestCase(0xCC)]
        [TestCase(0xDC)]
        public void Decode_CALLCC(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<CALLCC>(instruction);
        }

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
        public void Decode_LDR8R8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDR8R8>(instruction);
        }

        [TestCase(0x01)]
        [TestCase(0x11)]
        [TestCase(0x21)]
        [TestCase(0x31)]
        public void Decode_LDR16N16(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDR16D16>(instruction);
        }

        [Test]
        public void Decode_RLCA()
        {
            byte opcode = 0x07;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RLCA>(instruction);
        }

        [Test]
        public void Decode_RLA()
        {
            byte opcode = 0x17;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RLA>(instruction);
        }

        [Test]
        public void Decode_RRCA()
        {
            byte opcode = 0x0F;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RRCA>(instruction);
        }

        [Test]
        public void Decode_RRA()
        {
            byte opcode = 0x1F;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<RRA>(instruction);
        }

        [Test]
        public void Decode_LD_D16_SP()
        {
            byte opcode = 0x08;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_D16_SP>(instruction);
        }

        [Test]
        public void Decode_LDSPHL()
        {
            byte opcode = 0xF9;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDSPHL>(instruction);
        }

        [Test]
        public void Decode_ANDA_HL_()
        {
            byte opcode = 0xA6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ANDA_HL_>(instruction);
        }

        [Test]
        public void Decode_ORA_HL_()
        {
            byte opcode = 0xB6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ORA_HL_>(instruction);
        }

        [Test]
        public void Decode_XORA_HL_()
        {
            byte opcode = 0xAE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<XORA_HL_>(instruction);
        }

        [Test]
        public void Decode_ANDAD8()
        {
            byte opcode = 0xE6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ANDAD8>(instruction);
        }

        [Test]
        public void Decode_ORAD8()
        {
            byte opcode = 0xF6;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ORAD8>(instruction);
        }

        [Test]
        public void Decode_XORAD8()
        {
            byte opcode = 0xEE;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<XORAD8>(instruction);
        }

        [TestCase(0xA0)]
        [TestCase(0xA1)]
        [TestCase(0xA2)]
        [TestCase(0xA3)]
        [TestCase(0xA4)]
        [TestCase(0xA5)]
        [TestCase(0xA7)]
        public void Decode_ANDAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ANDAR8>(instruction);
        }

        [TestCase(0xB0)]
        [TestCase(0xB1)]
        [TestCase(0xB2)]
        [TestCase(0xB3)]
        [TestCase(0xB4)]
        [TestCase(0xB5)]
        [TestCase(0xB7)]
        public void Decode_ORAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<ORAR8>(instruction);
        }

        [TestCase(0xA8)]
        [TestCase(0xA9)]
        [TestCase(0xAA)]
        [TestCase(0xAB)]
        [TestCase(0xAC)]
        [TestCase(0xAD)]
        [TestCase(0xAF)]
        public void Decode_XORAR8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<XORAR8>(instruction);
        }

        [Test]
        public void Decode_LD_BC_A()
        {
            byte opcode = 0x02;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_BC_A>(instruction);
        }

        [Test]
        public void Decode_LD_DE_A()
        {
            byte opcode = 0x12;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_DE_A>(instruction);
        }

        [Test]
        public void Decode_LD_HLP_A()
        {
            byte opcode = 0x22;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_HLP_A>(instruction);
        }

        [Test]
        public void Decode_LD_HLM_A()
        {
            byte opcode = 0x32;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_HLM_A>(instruction);
        }

        [Test]
        public void Decode_LDA_BC_()
        {
            byte opcode = 0x0A;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDA_BC_>(instruction);
        }

        [Test]
        public void Decode_LDA_DE_()
        {
            byte opcode = 0x1A;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDA_DE_>(instruction);
        }

        [Test]
        public void Decode_LDA_HLP_()
        {
            byte opcode = 0x2A;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDA_HLP_>(instruction);
        }

        [Test]
        public void Decode_LDA_HLM_()
        {
            byte opcode = 0x3A;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDA_HLM_>(instruction);
        }

        [Test]
        public void Decode_LDH_D8_A()
        {
            byte opcode = 0xE0;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDH_D8_A>(instruction);
        }

        [Test]
        public void Decode_LDHA_D8_()
        {
            byte opcode = 0xF0;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDHA_D8_>(instruction);
        }

        [Test]
        public void Decode_LDH_C_A()
        {
            byte opcode = 0xE2;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDH_C_A>(instruction);
        }

        [Test]
        public void Decode_LDHA_C_()
        {
            byte opcode = 0xF2;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDHA_C_>(instruction);
        }

        [Test]
        public void Decode_LDHLSPD8()
        {
            byte opcode = 0xF8;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDHLSPS8>(instruction);
        }

        [Test]
        public void Decode_LD_D16_A()
        {
            byte opcode = 0xEA;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_D16_A>(instruction);
        }

        [Test]
        public void Decode_LDA_D16_()
        {
            byte opcode = 0xFA;

            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDA_D16_>(instruction);
        }

        [TestCase(0x70)]
        [TestCase(0x71)]
        [TestCase(0x72)]
        [TestCase(0x73)]
        [TestCase(0x74)]
        [TestCase(0x75)]
        [TestCase(0x77)]
        public void Decode_LD_HL_R8(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LD_HL_R8>(instruction);
        }

        [TestCase(0x46)]
        [TestCase(0x56)]
        [TestCase(0x66)]
        [TestCase(0x4E)]
        [TestCase(0x5E)]
        [TestCase(0x6E)]
        [TestCase(0x7E)]
        public void Decode_LDR8_HL_(byte opcode)
        {
            var instruction = _instructionDecoder.DecodeInstruction(opcode, false);

            Assert.IsInstanceOf<LDR8_HL_>(instruction);
        }

        [TestCase(0xD3)]
        [TestCase(0xE3)]
        [TestCase(0xE4)]
        [TestCase(0xF4)]
        [TestCase(0xDB)]
        [TestCase(0xEB)]
        [TestCase(0xEC)]
        [TestCase(0xFC)]
        [TestCase(0xDD)]
        [TestCase(0xED)]
        [TestCase(0xFD)]
        public void Decoding_forbidden_instructions_throws_exception(byte opcode)
        {
#if DEBUG
            //allow DEBUG instruction in debug mode
            if (opcode == 0xDD)
                return;
#endif
            Assert.Catch<InvalidOperationException>(() => _instructionDecoder.DecodeInstruction(opcode, false));
        }
    }
}

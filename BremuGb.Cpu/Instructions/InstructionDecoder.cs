using System;
using System.Collections.Generic;

using BremuGb.Cpu.Instructions;

namespace BremuGb.Cpu
{
    public class InstructionDecoder
    {
        private Dictionary<byte, IInstruction> _instructionCache;
        private Dictionary<byte, IInstruction> _prefixedInstructionCache;

        public InstructionDecoder()
        {
            _instructionCache = new Dictionary<byte, IInstruction>();
            _prefixedInstructionCache = new Dictionary<byte, IInstruction>();
        }

        public IInstruction DecodeInstruction(byte opcode, bool isPrefixed)
        {
            IInstruction instruction;

            if(!isPrefixed)
            {
                if (!_instructionCache.TryGetValue(opcode, out instruction))
                {
                    instruction = InstantiateInstruction(opcode);
                    _instructionCache.Add(opcode, instruction);
                }
            }
            else
            {
                if (!_prefixedInstructionCache.TryGetValue(opcode, out instruction))
                {
                    instruction = InstantiatePrefixedInstruction(opcode);
                    _prefixedInstructionCache.Add(opcode, instruction);
                }
            }

            instruction.Initialize(opcode);
            return instruction;
        }

        private IInstruction InstantiateInstruction(byte opcode)
        {
            if ((opcode >> 6) == 0x01 && (opcode & 0x07) != 0x06 && (opcode & 0x38) != 0x30)
                return new LDR8R8();

            if ((opcode & 0xC7) == 0x06 && opcode != 0x36)
                return new LDR8D8();

            if ((opcode & 0xF8) == 0x80 && opcode != 0x86)
                return new ADDAR8();

            if ((opcode & 0xF8) == 0x88 && opcode != 0x8E)
                return new ADCAR8();

            if ((opcode & 0xC7) == 0x04 && (opcode & 0x38) != 0x30)
                return new INCR8();

            if ((opcode & 0xC7) == 0x05 && (opcode & 0x38) != 0x30)
                return new DECR8();

            if ((opcode & 0xF8) == 0xA0 && opcode != 0xA6)
                return new ANDAR8();

            if ((opcode & 0xF8) == 0xB0 && opcode != 0xB6)
                return new ORAR8();

            if ((opcode & 0xF8) == 0xA8 && opcode != 0xAE)
                return new XORAR8();

            if ((opcode & 0xF8) == 0x70 && opcode != 0x76)
                return new LD_HL_R8();

            if ((opcode & 0xC7) == 0x46 && opcode != 0x76)
                return new LDR8_HL_();

            if ((opcode & 0xF8) == 0x90 && opcode != 0x96)
                return new SUBAR8();

            if ((opcode & 0xF8) == 0x98 && opcode != 0x9E)
                return new SBCAR8();

            if ((opcode & 0xF8) == 0xB8 && opcode != 0xBE)
                return new CPAR8();

            if ((opcode & 0xCF) == 0xC5)
                return new PUSH();

            if ((opcode & 0xCF) == 0xC1)
                return new POP();

            if ((opcode & 0xCF) == 0x03)
                return new INCR16();

            if ((opcode & 0xCF) == 0x0B)
                return new DECR16();

            if ((opcode & 0xC7) == 0xC7)
                return new RST();

            if ((opcode & 0xE7) == 0xC4)
                return new CALLCC();

            if ((opcode & 0xE7) == 0xC2)
                return new JPCC();

            if ((opcode & 0xE7) == 0xC0)
                return new RETCC();

            if ((opcode & 0xE7) == 0x20)
                return new JRCC();

            if ((opcode & 0xCF) == 0x01)
                return new LDR16D16();

            if ((opcode & 0xCF) == 0x09)
                return new ADDHLR16();

            switch (opcode)
            {
                case 0x00:
                    return new NOP();
                case 0x02:
                    return new LD_BC_A();
                case 0x07:
                    return new RLCA();
                case 0x08:
                    return new LD_D16_SP();
                case 0x0A:
                    return new LDA_BC_();
                case 0x0F:
                    return new RRCA();
                case 0x10:
                    return new STOP();
                case 0x12:
                    return new LD_DE_A();
                case 0x17:
                    return new RLA();
                case 0x18:
                    return new JR();
                case 0x1A:
                    return new LDA_DE_();
                case 0x1F:
                    return new RRA();
                case 0x22:
                    return new LD_HLP_A();
                case 0x27:
                    return new DAA();
                case 0x2A:
                    return new LDA_HLP_();
                case 0x2F:
                    return new CPL();
                case 0x32:
                    return new LD_HLM_A();
                case 0x34:
                    return new INC_HL_();
                case 0x35:
                    return new DEC_HL_();
                case 0x36:
                    return new LD_HL_D8();
                case 0x37:
                    return new SCF();
                case 0x3A:
                    return new LDA_HLM_();
                case 0x3F:
                    return new CCF();
                case 0x76:
                    return new HALT();
                case 0x86:
                    return new ADDA_HL_();
                case 0x8E:
                    return new ADCA_HL_();
                case 0x96:
                    return new SUBA_HL_();
                case 0x9E:
                    return new SBCA_HL_();
                case 0xA6:
                    return new ANDA_HL_();
                case 0xAE:
                    return new XORA_HL_();
                case 0xB6:
                    return new ORA_HL_();
                case 0xBE:
                    return new CPA_HL_();
                case 0xC3:
                    return new JPD16();
                case 0xC6:
                    return new ADDAD8();
                case 0xC9:
                    return new RET();
                case 0xCB:
                    return new PREFIX();
                case 0xCD:
                    return new CALL();
                case 0xCE:
                    return new ADCAD8();
                case 0xD6:
                    return new SUBAD8();
                case 0xD9:
                    return new RETI();
                case 0xE8:
                    return new ADDSPS8();
                case 0xDE:
                    return new SBCAD8();
                case 0xE0:
                    return new LDH_D8_A();
                case 0xE2:
                    return new LDH_C_A();
                case 0xE6:
                    return new ANDAD8();
                case 0xE9:
                    return new JPHL();
                case 0xEA:
                    return new LD_D16_A();
                case 0xEE:
                    return new XORAD8();
                case 0xF0:
                    return new LDHA_D8_();
                case 0xF2:
                    return new LDHA_C_();
                case 0xF3:
                    return new DI();
                case 0xF6:
                    return new ORAD8();
                case 0xF8:
                    return new LDHLSPS8();
                case 0xF9:
                    return new LDSPHL();
                case 0xFA:
                    return new LDA_D16_();
                case 0xFB:
                    return new EI();
                case 0xFE:
                    return new CPAD8();
#if DEBUG
                case 0xDD:
                    return new DEBUG();
#endif
                default:
                    throw new InvalidOperationException($"Unknown opcode, unable to decode: 0x{opcode:X2}");
            }
        }

        private IInstruction InstantiatePrefixedInstruction(byte opcode)
        {
            if ((opcode & 0xC0) == 0x40 && (opcode & 0x07) != 0x06)
                return new BITNR8();

            if ((opcode & 0xC0) == 0xC0 && (opcode & 0x07) != 0x06)
                return new SETNR8();

            if ((opcode & 0xC0) == 0x80 && (opcode & 0x07) != 0x06)
                return new RESNR8();

            if ((opcode & 0xC0) == 0x80 && (opcode & 0x07) == 0x06)
                return new RESN_HL_();

            if ((opcode & 0xC0) == 0xC0 && (opcode & 0x07) == 0x06)
                return new SETN_HL_();

            if ((opcode & 0xC0) == 0x40 && (opcode & 0x07) == 0x06)
                return new BITN_HL_();

            if ((opcode & 0xF8) == 0 && opcode != 0x06)
                return new RLCR8();

            if ((opcode & 0xF8) == 0x10 && opcode != 0x16)
                return new RLR8();

            if ((opcode & 0xF8) == 0x20 && opcode != 0x26)
                return new SLAR8();

            if ((opcode & 0xF8) == 0x30 && opcode != 0x36)
                return new SWAPR8();

            if ((opcode & 0xF8) == 0x08 && opcode != 0x0E)
                return new RRCR8();

            if ((opcode & 0xF8) == 0x18 && opcode != 0x1E)
                return new RRR8();

            if ((opcode & 0xF8) == 0x28 && opcode != 0x2E)
                return new SRAR8();

            if ((opcode & 0xF8) == 0x38 && opcode != 0x3E)
                return new SRLR8();

            switch(opcode)
            {
                case 0x06:
                    return new RLC_HL_();
                case 0x16:
                    return new RL_HL_();
                case 0x26:
                    return new SLA_HL_();
                case 0x36:
                    return new SWAP_HL_();
                case 0x0E:
                    return new RRC_HL_();
                case 0x1E:
                    return new RR_HL_();
                case 0x2E:
                    return new SRA_HL_();
                case 0x3E:
                    return new SRL_HL_();
                default:
                    throw new InvalidOperationException($"Unknown prefixed opcode, unable to decode: 0x{opcode:X2}");
            }            
        }        
    }
}

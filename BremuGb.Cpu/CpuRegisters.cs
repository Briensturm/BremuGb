using System;

namespace BremuGb.Cpu
{
    public class CpuRegisters
    {
        public byte F
        {
            get
            {
                return (byte)(((ZeroFlag ? 1 : 0) << 7) |
                              ((SubtractionFlag ? 1 : 0) << 6) |
                              ((HalfCarryFlag ? 1 : 0) << 5) |
                              ((CarryFlag ? 1 : 0) << 4));
            }

            set
            {
                ZeroFlag = (value & 0x80) == 0x80;
                SubtractionFlag = (value & 0x40) == 0x40;
                HalfCarryFlag = (value & 0x20) == 0x20;
                CarryFlag = (value & 0x10) == 0x10;
            }
        }
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public ushort BC
        {
            get
            {
                return (ushort)(B << 8 | C);
            }
            set
            {
                B = (byte)(value >> 8);
                C = (byte)(value & 0x00FF);
            }
        }

        public ushort DE
        {
            get
            {
                return (ushort)(D << 8 | E);
            }
            set
            {
                D = (byte)(value >> 8);
                E = (byte)(value & 0x00FF);
            }
        }

        public ushort HL
        {
            get
            {
                return (ushort)(H << 8 | L);
            }
            set
            {
                H = (byte)(value >> 8);
                L = (byte)(value & 0x00FF);
            }
        }

        public bool HalfCarryFlag { get; set; }
        public bool CarryFlag { get; set; }
        public bool ZeroFlag { get; set; }
        public bool SubtractionFlag { get; set; }

        public ushort this[int index]
        {
            get
            {
                return index switch
                {
                    0 => B,
                    1 => C,
                    2 => D,
                    3 => E,
                    4 => H,
                    5 => L,
                    6 => HL,
                    7 => A,
                    _ => throw new IndexOutOfRangeException("Cpu does not have a register with index " + index),
                };
            }
            set
            {
                switch(index)
                {
                    case 0:
                        B = (byte)value;
                        break;
                    case 1:
                        C = (byte)value;
                        break;
                    case 2:
                        D = (byte)value;
                        break;
                    case 3:
                        E = (byte)value;
                        break;
                    case 4:
                        H = (byte)value;
                        break;
                    case 5:
                        L = (byte)value;
                        break;
                    case 6:
                        HL = value;
                        break;
                    case 7:
                        A = (byte)value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Cpu does not have a register with index " + index);

                }
            }
        }

        public void Reset()
        {
            A = 0x01;            
            B = 0x00;
            C = 0x13;
            D = 0x00;
            E = 0xD8;
            H = 0x01;
            L = 0x4D;

            HalfCarryFlag = true;
            CarryFlag = true;
            ZeroFlag = true;
            SubtractionFlag = false;
        }
    }
}

using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC1 : MBCBase
    {
        private byte _romBankLower = 0x01;
        private byte _upperBits;

        private byte _bankingMode;

        public MBC1(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address > 0x7FFF)
                throw new InvalidOperationException($"MBC0: Memory read at out of bounds address 0x{address:X4}");

            if(address <= 0x3FFF)
                return _romData[address];

            if (_bankingMode == 0)
                return _romData[((_romBankLower | _upperBits) - 1) * 0x4000 + address];
            else if (_bankingMode == 0x01)
                return _romData[(_romBankLower - 1) * 0x4000 + address];
            else
                throw new InvalidOperationException();
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x3FFF)
            {
                if (data == 0)
                    _romBankLower = 0x01;
                else
                    _romBankLower = (byte)(data & 0x1F);
            }
            else if (address >= 0x4000 && address <= 0x5FFF)
            {
                _upperBits = (byte)((data & 0x03) << 5);
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                _bankingMode = data;
            }
            else
            {
                //TODO: Implement ram enable
            }  
        }
    }
}

using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC5 : MBCBase
    {
        private byte _romBankLower = 0x01;
        private int _upperBit;

        public MBC5(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address > 0x7FFF)
            {
                //TODO handle ram stuff
                return 0;
            }

            if(address <= 0x3FFF)
                return _romData[address];

            return _romData[((_romBankLower | (_upperBit << 8)) -1) * 0x4000 + address];
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x2FFF)
                _romBankLower = data;
            else if (address >= 0x3000 && address <= 0x3FFF)
                _upperBit = data & 0x01;
            else
            {
                //TODO: Implement ram stuff
            }  
        }
    }
}

using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC5 : MBCBase
    {
        private byte _romBankLower = 0x01;

        private int _upperBit;
        private int _ramBankNumber;

        private bool _ramEnable;        

        public MBC5(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x3FFF)
                return _romData[address];

            else if (address <= 0x7FFF)
                return _romData[((_romBankLower | (_upperBit << 8)) -1) * 0x4000 + address];

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return 0;

                return _ramData[address - 0xA000 + _ramBankNumber*0x2000];
            }

            throw new InvalidOperationException($"MBC5: Memory read at out of bounds address 0x{address:X4}");
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x2FFF)
                _romBankLower = data;

            else if (address >= 0x3000 && address <= 0x3FFF)
                _upperBit = data & 0x01;

            else if (address >= 0 && address <= 0x1FFF)
                _ramEnable = (data & 0xF) == 0xA;

            else if (address >= 0x4000 && address <= 0x5FFF)
                _ramBankNumber = data & 0xF;

            else if (address >= 0xA000 && address <= 0xBFFF)
                _ramData[address - 0xA000 + _ramBankNumber * 0x2000] = data;

            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                //read only memory, but some games write to this address range
            }

            else
                throw new InvalidOperationException($"{GetType().Name}: Memory write at out of bounds address 0x{address:X4}");
        }

        protected override bool CartridgeCanSave => _cartridgeType == CartridgeType.MBC5_RAM_BATTERY || 
                                                    _cartridgeType == CartridgeType.MBC5_RUMBLE_RAM_BATTERY;
    }
}

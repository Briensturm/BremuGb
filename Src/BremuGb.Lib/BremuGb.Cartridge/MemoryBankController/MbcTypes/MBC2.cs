using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC2 : MBCBase
    {
        private byte _romBankNumber = 0x01;

        private bool _ramEnable;

        public MBC2(byte[] romData) : base(romData)
        {
            _ramData = new byte[512];
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x3FFF)
                return _romData[address];

            else if (address <= 0x7FFF)
                return _romData[(_romBankNumber - 1) * 0x4000 + address];

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return 0xFF;

                return (byte)(_ramData[address & 0x01FF] | 0xF0);
            }

            throw new InvalidOperationException($"Memory read at out of bounds address 0x{address:X4}");
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address <= 0x3FFF)
            {
                if ((address & 0x0100) == 0)
                    _ramEnable = (data & 0xF) == 0xA;

                else
                {
                    _romBankNumber = (byte)(data & 0xF);

                    if (_romBankNumber == 0x0)
                        _romBankNumber = 0x01;

                    if (_romSizeType == RomSizeType.Rom_128KB)
                        _romBankNumber &= 0x07;
                    else if (_romSizeType == RomSizeType.Rom_64KB)
                        _romBankNumber &= 0x03;
                    else if (_romSizeType == RomSizeType.Rom_32KB)
                        _romBankNumber &= 0x01;                    
                }
            }

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return;
                
                _ramData[address & 0x01FF] = (byte)(data | 0xF0);
            }
        }

        protected override bool CartridgeCanSave => _cartridgeType == CartridgeType.MBC2_BATTERY;
    }
}

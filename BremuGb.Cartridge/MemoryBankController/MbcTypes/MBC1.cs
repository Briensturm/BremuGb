using System;

using BremuGb.Common.Constants;

namespace BremuGb.Cartridge.MemoryBankController
{
    //TODO: Multicart handling
    class MBC1 : MBCBase
    {
        private byte _romBankLower;
        private byte _upperBits;

        private bool _ramEnable;

        private int _bankingMode;

        private int RomBankNumber
        {
            get
            {
                int romBankNumber = _romBankLower;
                if (_bankingMode == 0 || _romSizeType >= RomSizeType.Rom_1MB)
                    romBankNumber |= _upperBits;

                //remap unusable banks
                if (romBankNumber == 0x0 || romBankNumber == 0x20 || romBankNumber == 0x40 || romBankNumber == 0x60)
                    romBankNumber++;

                //ignore some bits for smaller rom sizes
                if (_romSizeType == RomSizeType.Rom_32KB)
                    romBankNumber = 0x01;
                else if (_romSizeType == RomSizeType.Rom_64KB)
                    romBankNumber &= 0x03;
                else if (_romSizeType == RomSizeType.Rom_128KB)
                    romBankNumber &= 0x07;
                else if (_romSizeType == RomSizeType.Rom_256KB)
                    romBankNumber &= 0x0F;
                else if (_romSizeType == RomSizeType.Rom_512KB)
                    romBankNumber &= 0x1F;
                else if (_romSizeType == RomSizeType.Rom_1MB)
                    romBankNumber &= 0x3F;                

                return romBankNumber;
            }
        }

        public MBC1(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= CartridgeConstants.FirstRomBankAddressEnd)
            {
                if (_romSizeType > RomSizeType.Rom_1MB && _bankingMode == 1)
                    return _romData[_upperBits * CartridgeConstants.RomBankSize + address];
                if (_romSizeType == RomSizeType.Rom_1MB && _bankingMode == 1)
                    return _romData[(_upperBits & 0x20) * CartridgeConstants.RomBankSize + address];
                else
                    return _romData[address];
            }

            else if (address <= CartridgeConstants.RomAddressEnd)
                return _romData[(RomBankNumber - 1) * CartridgeConstants.RomBankSize + address];

            else if (address >= CartridgeConstants.CartRamAddressBegin && address <= CartridgeConstants.CartRamAddressEnd)
            {
                if (_ramSizeType == RamSizeType.None || !_ramEnable)
                    return 0xFF;

                //ram address range of 2KB ram carts ends with address 0xA7FF
                if (address > 0xA7FF && _ramSizeType < RamSizeType.Ram_8KB)
                    return 0xFF;

                //use upper bits for ram bank number only if banking mode is non-zero
                else if (_bankingMode == 0)
                    return _ramData[address - CartridgeConstants.CartRamAddressBegin];
                else
                {
                    var ramBankNumber = _upperBits >> 5;
                    if (_ramSizeType == RamSizeType.Ram_8KB)
                        ramBankNumber = 0;

                    return _ramData[ramBankNumber * 0x2000 + address - 0xA000];
                }
            }

            throw new InvalidOperationException($"Memory read at out of bounds address 0x{address:X4}");
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x3FFF)
                _romBankLower = (byte)(data & 0x1F);

            else if (address >= 0x4000 && address <= 0x5FFF)
                _upperBits = (byte)((data & 0x03) << 5);

            else if (address >= 0x6000 && address <= 0x7FFF)
                _bankingMode = data & 0x01;

            else if(address >= 0 && address <= 0x1FFF)
                _ramEnable = (data & 0xF) == 0xA;

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (_ramSizeType == RamSizeType.None || !_ramEnable)
                    return;

                //ram address range of 2KB ram carts ends with address 0xA7FF
                if (address > 0xA7FF && _ramSizeType < RamSizeType.Ram_8KB)
                    return;

                if (_bankingMode == 0)
                    _ramData[address - 0xA000] = data;
                else
                {
                    var ramBankNumber = _upperBits >> 5;
                    if (_ramSizeType == RamSizeType.Ram_8KB)
                        ramBankNumber = 0;

                    _ramData[ramBankNumber * 0x2000 + address - 0xA000] = data;
                }
            }

            else
                throw new InvalidOperationException($"Memory write at out of bounds address 0x{address:X4}");
        }

        protected override bool CartridgeCanSave => _cartridgeType == CartridgeType.MBC1_RAM_BATTERY;
    }
}

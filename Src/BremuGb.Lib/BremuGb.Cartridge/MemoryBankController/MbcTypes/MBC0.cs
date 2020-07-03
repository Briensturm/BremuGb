using System;

using BremuGb.Common.Constants;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC0 : MBCBase
    {
        public MBC0(byte[] romData) : base(romData)
        {            
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= CartridgeConstants.RomAddressEnd)
                return _romData[address];

            else if (address >= CartridgeConstants.CartRamAddressBegin && address <= CartridgeConstants.CartRamAddressEnd)
                return _ramData[CartridgeConstants.CartRamAddressBegin - address];

            else
                throw new InvalidOperationException($"{GetType().Name}: Memory read at out of bounds address 0x{address:X4}");            
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= CartridgeConstants.CartRamAddressBegin && address <= CartridgeConstants.CartRamAddressEnd)
                _ramData[CartridgeConstants.CartRamAddressBegin - address] = data;
        }        

        protected override bool CartridgeCanSave => _cartridgeType == CartridgeType.ROM_RAM_BATTERY;
    }
}

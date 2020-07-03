using System;

using BremuGb.Common.Constants;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC3 : MBCBase
    {
        private byte _romBankNumber = 0x01;

        private byte _seconds;
        private byte _minutes;
        private byte _hours;
        private int _days;

        private bool _ramEnable;
        private bool _rtcHalt;        
        private bool _prepareLatch;

        private int _ramBankNumber;

        //TODO: Proper RTC
        public MBC3(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            //fixed first bank
            if (address <= CartridgeConstants.FirstRomBankAddressEnd)
                return _romData[address];

            //bank selectable via rom bank number
            else if (address <= CartridgeConstants.RomAddressEnd)
                return _romData[(_romBankNumber - 1) * CartridgeConstants.RomBankSize + address];

            //ram access
            else if(address >= CartridgeConstants.CartRamAddressBegin && address <= CartridgeConstants.CartRamAddressEnd)
            {
                if (!_ramEnable)
                    return 0;

                if(_ramBankNumber >= 0 && _ramBankNumber <= 3)
                    return _ramData[address - CartridgeConstants.CartRamAddressBegin + CartridgeConstants.RamBankSize*_ramBankNumber];

                switch(_ramBankNumber)
                {
                    case 0x08:
                        return _seconds;
                    case 0x09:
                        return _minutes;
                    case 0x0A:
                        return _hours;
                    case 0x0B:
                        return (byte)_days;
                    case 0x0C:
                        return (byte)((_days & 0x100) | (_rtcHalt ? 0x40 : 0x00));
                }

                return 0;
            }

            throw new InvalidOperationException($"{GetType().Name}: Memory read at out of bounds address 0x{address:X4}");
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x3FFF)
            {
                if (data == 0)
                    _romBankNumber = 0x01;
                else if(data <= 0x7F)
                    _romBankNumber = data;                
            }

            else if (address >= 0x4000 && address <= 0x5FFF)
                _ramBankNumber = data;

            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                if (!_prepareLatch && data == 0x00)
                    _prepareLatch = true;
                else if (_prepareLatch && data == 0x01)
                {
                    //latch realtime
                    _seconds = (byte)DateTime.Now.Second;
                    _minutes = (byte)DateTime.Now.Minute;
                    _hours = (byte)DateTime.Now.Hour;
                    _days = DateTime.Now.DayOfYear;

                    _prepareLatch = false;
                }
                else
                    _prepareLatch = false;
            }

            else if (address >= 0 && address <= 0x1FFF)
                _ramEnable = (data & 0xF) == 0xA;

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return;

                if (_ramBankNumber >= 0 && _ramBankNumber <= 3)
                    _ramData[address - 0xA000 + 0x2000 * _ramBankNumber] = data;

                else if (_ramBankNumber == 0x0C)
                    _rtcHalt = (data & 0x40) == 0x40;
            }            
        }

        protected override bool CartridgeCanSave => _cartridgeType == CartridgeType.MBC3_TIMER_BATTERY ||
                                                    _cartridgeType == CartridgeType.MBC3_RAM_BATTERY ||
                                                    _cartridgeType == CartridgeType.MBC3_TIMER_RAM_BATTERY;
    }
}

using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC3 : MBCBase
    {
        private byte _romBankNumber = 0x01;

        private byte _seconds;
        private byte _minutes;
        private byte _hours;
        private int _days;

        private bool _rtcHalt;

        private bool _ramEnable;
        private byte[] _ramData;

        private bool _prepareLatch;

        private int _ramBankNumber;

        public MBC3(byte[] romData) : base(romData)
        {
            _ramData = new byte[0x8000];
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x3FFF)
                return _romData[address];

            else if (address <= 0x7FFF)
                return _romData[(_romBankNumber - 1) * 0x4000 + address];

            else if(address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return 0;

                if(_ramBankNumber >= 0 && _ramBankNumber <= 3)
                    return _ramData[address - 0xA000 + 0x2000*_ramBankNumber];

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

            throw new InvalidOperationException($"MBC3: Memory read at out of bounds address 0x{address:X4}");
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

        public override void LoadRam(IRamManager ramManager)
        {
            if(CartridgeCanSave())
                _ramData = ramManager.LoadRam();
        }

        public override void SaveRam(IRamManager ramManager)
        {
            if (CartridgeCanSave())
                ramManager.SaveRam(_ramData);
        }

        private bool CartridgeCanSave()
        {
            return _cartridgeType == 0x10 || _cartridgeType == 0x13;
        }
    }
}

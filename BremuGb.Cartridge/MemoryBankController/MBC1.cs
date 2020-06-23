using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC1 : MBCBase
    {
        private byte _romBankLower = 0x01;
        private byte _upperBits;

        private bool _ramEnable;
        private byte[] _ramData;

        private int _bankingMode;

        public MBC1(byte[] romData) : base(romData)
        {
            _ramData = new byte[0x8000];
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x3FFF)
                return _romData[address];

            else if (address <= 0x7FFF)
            {
                if (_bankingMode == 0)
                    return _romData[((_romBankLower | _upperBits) - 1) * 0x4000 + address];
                else
                    return _romData[(_romBankLower - 1) * 0x4000 + address];
            }

            else if(address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return 0;

                else if (_bankingMode == 0)
                    return _ramData[address - 0xA000];
                else
                    return _ramData[(_upperBits >> 5)*0x2000 + address - 0xA000];
            }

            throw new InvalidOperationException($"MBC1: Memory read at out of bounds address 0x{address:X4}");
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
                _upperBits = (byte)((data & 0x03) << 5);

            else if (address >= 0x6000 && address <= 0x7FFF)
                _bankingMode = data & 0x01;

            else if(address >= 0 && address <= 0x1FFF)
                _ramEnable = (data & 0xF) == 0xA;

            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (!_ramEnable)
                    return;

                if (_bankingMode == 0)
                    _ramData[address - 0xA000] = data;
                else
                    _ramData[(_upperBits >> 5) * 0x2000 + address - 0xA000] = data;
            }

            else
                throw new InvalidOperationException($"MBC1: Memory write at out of bounds address 0x{address:X4}");
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
            return _cartridgeType == 0x03;
        }
    }
}

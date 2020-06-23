using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC2 : MBCBase
    {
        private byte _romBankNumber = 0x01;

        private bool _ramEnable;
        private byte[] _ramData;

        public MBC2(byte[] romData) : base(romData)
        {
            _ramData = new byte[0x0200];
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x3FFF)
                return _romData[address];

            else if (address <= 0x7FFF)
                return _romData[(_romBankNumber - 1) * 0x4000 + address];

            else if(address >= 0xA000 && address <= 0xA1FF)
            {
                if (!_ramEnable)
                    return 0;

                return _ramData[address - 0xA000];
            }

            throw new InvalidOperationException($"MBC2: Memory read at out of bounds address 0x{address:X4}");
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x2000 && address <= 0x3FFF)
            {
                if ((address & 0x0100) != 0x0100)
                    return;

                if (data == 0)
                    _romBankNumber = 0x01;
                else
                    _romBankNumber = (byte)(data & 0xF);
            }

            else if(address >= 0 && address <= 0x1FFF)
            {
                if ((address & 0x0100) == 0x0)
                    _ramEnable = !_ramEnable;
            }                

            else if (address >= 0xA000 && address <= 0xA1FF)
            {
                if (!_ramEnable)
                    return;
                
                _ramData[address - 0xA000] = (byte)(data | 0xF0);
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
            return _cartridgeType == 0x06;
        }
    }
}

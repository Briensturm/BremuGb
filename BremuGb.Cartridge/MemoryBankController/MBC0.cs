using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC0 : MBCBase
    {
        private byte[] _ramData;

        public MBC0(byte[] romData) : base(romData)
        {
            _ramData = new byte[0x2000];
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address <= 0x7FFF)
                return _romData[address];

            else if (address >= 0xA000 && address <= 0xBFFF)
                return _ramData[0xA000 - address];

            else
                throw new InvalidOperationException($"MBC0: Memory read at out of bounds address 0x{address:X4}");            
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0xA000 && address <= 0xBFFF)
                _ramData[0xA000 - address] = data;
        }

        public override void LoadRam(IRamManager ramManager)
        {
            if (CartridgeCanSave())
                _ramData = ramManager.LoadRam();
        }

        public override void SaveRam(IRamManager ramManager)
        {
            if (CartridgeCanSave())
                ramManager.SaveRam(_ramData);
        }

        private bool CartridgeCanSave()
        {
            return _cartridgeType == 0x09;
        }
    }
}

using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    class MBC0 : MBCBase
    {
        public MBC0(byte[] romData) : base(romData)
        {
        }

        public override byte DelegateMemoryRead(ushort address)
        {
            if (address > 0x7FFF)
                throw new InvalidOperationException($"MBC0: Memory read at out of bounds address 0x{address:X4}");

            return _romData[address];
        }

        public override void DelegateMemoryWrite(ushort address, byte data)
        {
            throw new System.NotImplementedException("MBC0: Memory write not implemented for now");
        }
    }
}

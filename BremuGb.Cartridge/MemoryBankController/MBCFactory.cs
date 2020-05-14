using System;
using System.Collections.Generic;
using System.Text;

namespace BremuGb.Cartridge.MemoryBankController
{
    public static class MBCFactory
    {
        public static IMemoryBankController CreateMBC(IRomLoader romLoader)
        {
            var romData = romLoader.LoadRom();
            var mbcType = romData[0x0147];

            switch (mbcType)
            {
                case 0x00:
                    return new MBC0(romData);
                default:
                    throw new InvalidOperationException($"MBC type 0x{mbcType:X2} is not supported");
            }
        }
    }
}

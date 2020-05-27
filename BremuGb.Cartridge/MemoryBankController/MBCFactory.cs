using System;

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
                case 0x01:
                case 0x02:
                case 0x03:
                    return new MBC1(romData);
                default:
                    throw new NotSupportedException($"MBC type 0x{mbcType:X2} is not supported");
            }
        }
    }
}

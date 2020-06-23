using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    public static class MBCFactory
    {
        public static IMemoryBankController CreateMBC(IRomLoader romLoader)
        {
            var romData = romLoader.LoadRom();
            var cartridgeType = romData[0x0147];

            switch (cartridgeType)
            {
                case 0x00:
                    return new MBC0(romData);
                case 0x01:
                case 0x02:
                case 0x03:
                    return new MBC1(romData);
                case 0x05:
                case 0x06:
                    return new MBC2(romData);
                case 0x1B:
                    return new MBC5(romData);
                default:
                    throw new NotSupportedException($"MBC type 0x{cartridgeType:X2} is not supported");
            }
        }
    }
}

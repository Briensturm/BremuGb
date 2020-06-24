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
                case 0x08:
                case 0x09:
                    return new MBC0(romData);
                case 0x01:
                case 0x02:
                case 0x03:
                    return new MBC1(romData);
                case 0x05:
                case 0x06:
                    return new MBC2(romData);
                case 0x0F:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                    return new MBC3(romData);
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                    return new MBC5(romData);
                default:
                    throw new NotSupportedException($"Cartridge type 0x{cartridgeType:X2} is not supported");
            }
        }
    }
}
